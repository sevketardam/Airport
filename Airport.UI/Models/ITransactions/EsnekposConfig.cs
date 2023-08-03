using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Airport.UI.Models.ITransactions
{
    public class EsnekposProgram
    {
        public string Name { get; set; }
        public string Bank { get; set; }
        public bool Installments { get; set; }
    }

    public class EsnekposInstallment
    {
        public double Value { get; set; }
        public int Active { get; set; }
    }

    public class PayConfig
    {
        public string MERCHANT { get; set; }
        public string MERCHANT_KEY { get; set; }
        public string BACK_URL { get; set; }
        public string PRICES_CURRENCY { get; set; }
        public string ORDER_REF_NUMBER { get; set; }
        public string ORDER_AMOUNT { get; set; }
    }

    public class PayCard
    {
        public string CC_NUMBER { get; set; }
        public string EXP_MONTH { get; set; }
        public string EXP_YEAR { get; set; }
        public string CC_CVV { get; set; }
        public string CC_OWNER { get; set; }
        public string INSTALLMENT_NUMBER { get; set; }
    }

    public class PayCustomer
    {
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MAIL { get; set; }
        public string PHONE { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string ADDRESS { get; set; }
        public string CLIENT_IP { get; set; }
    }

    public class PayProduct
    {
        public string PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_CATEGORY { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public string PRODUCT_AMOUNT { get; set; }
    }

    public class PayDetail
    {
        public PayConfig Config { get; set; }
        public PayCard CreditCard { get; set; }
        public PayCustomer Customer { get; set; }
        public List<PayProduct> Product { get; set; } = new List<PayProduct>();

        public string HASH { get; set; }
        public string POST_URL { get; set; }
    }

    public class ReturnPayment
    {
        public string ORDER_REF_NUMBER { get; set; }
        public string STATUS { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_MESSAGE_TR { get; set; }
        public string ERROR_CODE { get; set; }
        public string DATE { get; set; }
        public string URL_3DS { get; set; }
        public string REFNO { get; set; }
        public string HASH { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_MAIL { get; set; }
        public string CUSTOMER_PHONE { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string CUSTOMER_CC_NUMBER { get; set; }
        public string CUSTOMER_CC_NAME { get; set; }
        public bool IS_NOT_3D_PAYMENT { get; set; }
        public string VIRTUAL_POS_VALUES { get; set; }
        public string RETURN_MESSAGE_3D { get; set; }
        public string BANK_AUTH_CODE { get; set; }
    }

    public class Return3DPayment
    {
        public string DATE { get; set; }
        public string HASH { get; set; }
        public string ORDER_REF_NUMBER { get; set; }
        public string REFNO { get; set; }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_MESSAGE_TR { get; set; }
        public string STATUS { get; set; }
        public string ERROR_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_MAIL { get; set; }
        public string CUSTOMER_PHONE { get; set; }
        public string CUSTOMER_ADDRESS { get; set; }
        public string CUSTOMER_CC_NUMBER { get; set; }
        public string CUSTOMER_CC_NAME { get; set; }
        public string COMMISSION { get; set; }
        public string COMMISSION_RATE { get; set; }
        public string AMOUNT { get; set; }
        public string INSTALLMENT { get; set; }
        public string BANK_AUTH_CODE { get; set; }
    }

    public class EsnekposConfig
    {
        public const int MaxInstallment = 12;

        public static List<EsnekposProgram> GetAvailablePrograms()
        {
            return new List<EsnekposProgram>
        {
            new EsnekposProgram { Name = "Axess", Bank = "Akbank A.Ş.", Installments = true },
            new EsnekposProgram { Name = "WordCard", Bank = "Yapı Kredi Bankası", Installments = true },
            new EsnekposProgram { Name = "BonusCard", Bank = "Garanti Bankası A.Ş.", Installments = true },
            new EsnekposProgram { Name = "CardFinans", Bank = "FinansBank A.Ş.", Installments = true },
            new EsnekposProgram { Name = "Maximum", Bank = "T.C. İş Bankası", Installments = true },
            new EsnekposProgram { Name = "Paraf", Bank = "Halk Bankası", Installments = true },
            new EsnekposProgram { Name = "Combo", Bank = "Ziraat Bankası", Installments = true }
        };
        }

        public static string GetCheckKey(string dealerCode, string username, string password)
        {
            string concatenatedString = $"{dealerCode}MK{username}PD{password}";
            byte[] byteArray = Encoding.UTF8.GetBytes(concatenatedString);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(byteArray);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static List<EsnekposInstallment> SetRatesFromPost(List<EsnekposInstallment> postedData, string bankName)
        {
            var banks = GetAvailablePrograms();
            var result = new List<EsnekposInstallment>();

            foreach (var bank in banks)
            {
                for (int i = 1; i <= MaxInstallment; i++)
                {
                    var installment = new EsnekposInstallment
                    {
                        Value = postedData.Find(data => bankName == bank.Name && data.Active == i)?.Value ?? 0.0,
                        Active = postedData.Find(data => bankName == bank.Name && data.Active == i)?.Active ?? 0
                    };
                    result.Add(installment);
                }
            }

            return result;
        }

        public static List<EsnekposInstallment> SetRatesDefault()
        {
            var banks = GetAvailablePrograms();
            var result = new List<EsnekposInstallment>();

            foreach (var bank in banks)
            {
                for (int i = 1; i <= MaxInstallment; i++)
                {
                    var installment = new EsnekposInstallment
                    {
                        Value = (i == 1) ? 0.00 : 1 + i + (i / 5) + 0.1,
                        Active = bank.Installments ? 1 : 0
                    };
                    result.Add(installment);
                }
            }

            return result;
        }

        public static List<EsnekposInstallment> SetRatesNull()
        {
            var banks = GetAvailablePrograms();
            var result = new List<EsnekposInstallment>();

            foreach (var bank in banks)
            {
                for (int i = 1; i <= MaxInstallment; i++)
                {
                    var installment = new EsnekposInstallment
                    {
                        Value = 0,
                        Active = 0
                    };
                    result.Add(installment);
                }
            }

            return result;
        }


        // The rest of the methods can be similarly refactored using lists of custom classes.

        // Note: For the HTML table generation, you can use C#'s StringBuilder or other approaches.
    }
}