using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1005.Areas.InsurancesandFund.Models
{
    public class IRRViewModel
    {
        public DateTime PurchaseDate { get; set; }
        public DateTime WithdrawDate { get; set; }
        public int PayYear { get; set; }
        public int PaymentPerYear { get; set; }
        public int Withdrawal { get; set; }

    }
    public class IRRCalculater
    {
        public string IRR(IRRViewModel data)
        {
            int TotalYears = 0;
            if (data.PurchaseDate.Month > data.WithdrawDate.Month)
            {
                TotalYears = data.WithdrawDate.Year - data.PurchaseDate.Year;
            }
            else if (data.PurchaseDate.Month == data.WithdrawDate.Month)
            {
                if (data.PurchaseDate.Date <= data.WithdrawDate.Date)
                {
                    TotalYears = data.WithdrawDate.Year - data.PurchaseDate.Year + 1;
                }
                else
                {
                    TotalYears = data.WithdrawDate.Year - data.PurchaseDate.Year;
                }
            }
            else
            {
                TotalYears = data.WithdrawDate.Year - data.PurchaseDate.Year + 1;
            }

            double[] cashflow = new double[TotalYears];
            for (int i = 0; i < TotalYears; i++)
            {
                cashflow[i] = 0;
            }
            for (int i = 0; i < data.PayYear; i++)
            {
                cashflow[i] = -data.PaymentPerYear;
            }
            cashflow[cashflow.Length - 1] = data.Withdrawal;
            return Financial.IRR(ref cashflow).ToString("P");
        }
    }
}