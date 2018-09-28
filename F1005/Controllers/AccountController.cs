using F1005.Classes;
using F1005.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace F1005.Controllers
{
    public class AccountController : Controller
    {
        // GET: Acoount 登入
        public ActionResult Login()
        {       
            string code = CreateCheckCodeImage.GenerateCheckCode();
            MemoryStream ms = CreateCheckCodeImage.Production(code);
            byte[] buffurPic = ms.ToArray();

            //轉換二進位檔
            string imreBase64Data = Convert.ToBase64String(buffurPic);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            //Passing image data in viewbag to view
            TempData["img"] = imgDataURL;

            Session["cc"] = code;

            return View();
        }

        public ActionResult ChangeImg()
        {
            string code = CreateCheckCodeImage.GenerateCheckCode();
            MemoryStream ms = CreateCheckCodeImage.Production(code);
            byte[] buffurPic = ms.ToArray();

            //轉換二進位檔
            string imreBase64Data = Convert.ToBase64String(buffurPic);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            //Passing image data in viewbag to view
            TempData["img"] = imgDataURL;

            Session["cc"] = code;

            return Content(imgDataURL);
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //if (String.IsNullOrEmpty(model.UserName))
            //{
            //    ModelState.AddModelError("UserName", "不可為空");
            //}

            //if (String.IsNullOrEmpty(model.Password))
            //{
            //    ModelState.AddModelError("Password", "不可為空");
            //}

            MyInvestEntities db = new MyInvestEntities();

            //登入密碼雜湊
            byte[] enc = System.Text.Encoding.Default.GetBytes(model.Password);
            HashAlgorithm ha = HashAlgorithm.Create("SHA256");
            byte[] ans = ha.ComputeHash(enc);

            model.Password = BitConverter.ToString(ans);

            var loginer = db.UsersData.Where(c => model.UserName == c.UserName && model.Password == c.Password).FirstOrDefault();

            var inputcc = model.CheckCode.ToUpper();

            if (Session["cc"].ToString() != inputcc)
            {
                TempData["status"] = "登入碼輸入錯誤";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (loginer != null)
                {
                    if (model.UserName == "msit119")
                    {
                        Session["User"] = model.UserName;
                        return RedirectToAction("Admin");
                    }
                    else
                    {
                        Session["User"] = model.UserName;
                        //return RedirectToAction("About","Home", new { Area = "Main"});
                        return RedirectToRoute("Main_default", new { Controller = "Home", Action = "Index" });
                    }
                }
                else
                {
                    TempData["status"] = "登入失敗";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        // POST: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UsersData model)
        {
            MyInvestEntities db = new MyInvestEntities();

            var member = db.UsersData.Where(c => c.UserName == model.UserName || c.Email == model.Email).FirstOrDefault();

            //if (member == null)
            //{
                //雜湊
                byte[] enc = System.Text.Encoding.Default.GetBytes(model.Password);
                HashAlgorithm ha = HashAlgorithm.Create("SHA256");
                byte[] ans = ha.ComputeHash(enc);

                model.Password = BitConverter.ToString(ans);

                //將會員記錄新增到Users資料表
                db.UsersData.Add(model);

                //儲存資料庫變更
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {

                    throw ex;
                }

                Session["User"] = model.UserName;

                //執行Home控制器的Login動作方法
                return RedirectToRoute("Main_default", new { Controller = "Home", Action = "Index" });
            //}
            //ViewBag.Message = "此帳號或電子郵件己有人使用，註冊失敗";
            //return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(RegisterViewModel model)
        {
            MyInvestEntities db = new MyInvestEntities();

            var nr = db.UsersData.Where(c => model.UserName == c.UserName || model.Email == c.Email);
            if (nr.Count() > 0)
            {
                return Content("Existed");
            }
            else
            {
                Random r = new Random();

                string code = "";

                for (int i = 0; i < 5; ++i)
                    switch (r.Next(0, 3))
                    {
                        //數字0~9
                        case 0: code += r.Next(0, 10); break;
                        //小寫a-z
                        case 1: code += (char)r.Next(65, 91); break;
                        //大寫A-Z
                        case 2: code += (char)r.Next(97, 122); break;
                    }

                cCode = code;

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.To.Add(model.Email);
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
                //這裡可以隨便填，不是很重要
                msg.From = new MailAddress("xxx@gmail.com", "$$$", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = "測試標題";//郵件標題
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
                msg.Body = "驗證碼為:" + "<span style='color:red'>" + code + "</span>";
                //郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                                                             //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                msg.IsBodyHtml = true;//是否是HTML郵件 
                                      //msg.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("msit119.finalterm@gmail.com", "msit1199"); //這裡要填正確的帳號跟密碼
                client.Host = "smtp.gmail.com"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = true; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();


                System.Timers.Timer timer = new System.Timers.Timer();

                timer.Enabled = true;
                timer.Interval = 3000000; // 5分鐘後驗證碼失效
                timer.Start();
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = false;

                //return RedirectToAction("ConfirmEmail", "Account");

                return Content(code);
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cCode = null;
        }

        public static string cCode { get; set; }

        [HttpPost]
        public ActionResult ConfirmEmailx(RegisterViewModel model)
        {
            if (model.Code == cCode && model.Code != null)
            {
                return Content("Cor");
            }

            return Content("Err");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            MyInvestEntities db = new MyInvestEntities();

            var item = db.UsersData.Where(c => (c.UserName == model.UserName && c.Email == model.Email)).FirstOrDefault();

            if (item != null)
            {
                Random r = new Random();

                string code = "";

                for (int i = 0; i < 10; ++i)
                    switch (r.Next(0, 3))
                    {
                        //數字0~9
                        case 0: code += r.Next(0, 10); break;
                        //小寫a-z
                        case 1: code += (char)r.Next(65, 91); break;
                        //大寫A-Z
                        case 2: code += (char)r.Next(97, 122); break;
                    }

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.To.Add(model.Email);
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
                //這裡可以隨便填，不是很重要
                msg.From = new MailAddress("xxx@gmail.com", "$$$", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = "測試標題";//郵件標題
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
                msg.Body = $"新密碼為:<span style='color:red'>{code}</span><br> 請依此登入重新設定密碼!";
                //郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                                                             //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                msg.IsBodyHtml = true;//是否是HTML郵件 
                                      //msg.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("msit119.finalterm@gmail.com", "msit1199"); //這裡要填正確的帳號跟密碼
                client.Host = "smtp.gmail.com"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = true; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();

                //System.Timers.Timer timer = new System.Timers.Timer();

                //timer.Enabled = true;
                //timer.Interval = 3000000; // 5分鐘後驗證碼失效
                //timer.Start();
                //timer.Elapsed += Timer_Elapsed;
                //timer.AutoReset = false;

                //return RedirectToAction("ConfirmEmail","Account");                     

                byte[] enc = System.Text.Encoding.Default.GetBytes(code);
                HashAlgorithm ha = HashAlgorithm.Create("SHA256");
                byte[] ans = ha.ComputeHash(enc);

                item.Password = BitConverter.ToString(ans);

                //item.Password = code;

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    throw ex;
                }
                TempData["Request"] = "已於Email寄出新密碼";
                TempData["Request2"] = "請以新密碼登入並修改密碼";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.Message = "查無此人，請確認使用者名稱或Email正確";
                //return View("Index2");
                return View();
            }
        }

        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            string u = Session["User"] as string;
            if (String.IsNullOrEmpty(u))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        // POST: /Account/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            MyInvestEntities db = new MyInvestEntities();

            byte[] enc = System.Text.Encoding.Default.GetBytes(model.OPassword);
            HashAlgorithm ha = HashAlgorithm.Create("SHA256");
            byte[] ans = ha.ComputeHash(enc);

            model.OPassword = BitConverter.ToString(ans);

            var item = db.UsersData.Where(c => c.Password == model.OPassword).FirstOrDefault();


            if (item != null)
            {
                byte[] nenc = System.Text.Encoding.Default.GetBytes(model.Password);
                byte[] nans = ha.ComputeHash(nenc);

                model.Password = BitConverter.ToString(nans);

                item.Password = model.Password;
                item.RememberMe = true;

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {

                    throw ex;
                }

                return RedirectToRoute("Main_default", new { Controller = "Home", Action = "Index" });
            }
            else
            {
                ViewBag.Message = "原密碼錯誤";
                return View("Index2");
            }
        }

        //SHOW出現有登入者
        MyInvestEntities db = new MyInvestEntities();
        // GET: /Account/Show
        [AllowAnonymous]
        public ActionResult Show()
        {
            return View(db.UsersData);
        }

        public ActionResult Logout()
        {
            Session["User"] = null;

            return RedirectToAction("Index", "Home");
            //return View("Index","Home");
        }

        public ActionResult LoginGoogle()
        {
            return View();
        }


        //public ActionResult Yes()
        //{
        //    string u = Session["User"] as string;
        //    if (String.IsNullOrEmpty(u))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View();
        //    //return View("Index","Home");
        //}

        public ActionResult Admin()
        {
            string u = Session["User"] as string;
            if (String.IsNullOrEmpty(u))
            {
                return RedirectToAction("Index", "Home");
            }
            var data = db.UsersData.Select(c => new LoginViewModel
            {
                UserName = c.UserName,
                Password = c.Password,
                Email = c.Email
            });

            return View(data);
        }

        public ActionResult ChangePasswordAdmin()
        {
            string u = Session["User"] as string;
            if (String.IsNullOrEmpty(u))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePasswordAdmin(ChangePasswordViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            MyInvestEntities db = new MyInvestEntities();

            byte[] enc = System.Text.Encoding.Default.GetBytes(model.OPassword);
            HashAlgorithm ha = HashAlgorithm.Create("SHA256");
            byte[] ans = ha.ComputeHash(enc);

            model.OPassword = BitConverter.ToString(ans);

            var item = db.UsersData.Where(c => c.Password == model.OPassword).FirstOrDefault();


            if (item != null)
            {
                byte[] nenc = System.Text.Encoding.Default.GetBytes(model.Password);
                byte[] nans = ha.ComputeHash(nenc);

                model.Password = BitConverter.ToString(nans);

                item.Password = model.Password;
                item.RememberMe = true;

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {

                    throw ex;
                }

                return RedirectToAction("Admin", "Account");
            }
            else
            {
                ViewBag.Message = "原密碼錯誤";
                return View("Index2");
            }
        }
    }
}
