using System;
using System.Collections.Generic;

namespace Airport.UI.Models.ITransactions
{
    //public class Bank
    //{
    //    public string Name { get; set; }
    //    public string BankName { get; set; }
    //    public bool Installments { get; set; }
    //}

    //public class Installment
    //{
    //    public float Value { get; set; }
    //    public int Active { get; set; }
    //}

    //public class BankRates
    //{
    //    public int Active { get; set; }
    //    public List<Installment> Installments { get; set; }
    //}

    public class EsnekposConfig
    {
        //public readonly int MaxInstallment = 12;

        //private List<Bank> GetAvailablePrograms()
        //{
        //    return new List<Bank>
        //{
        //    new Bank { Name = "axess", BankName = "Akbank A.Ş.", Installments = true },
        //    new Bank { Name = "world", BankName = "Yapı Kredi Bankası", Installments = true },
        //    new Bank { Name = "bonus", BankName = "Garanti Bankası A.Ş.", Installments = true },
        //    new Bank { Name = "cardfinans", BankName = "FinansBank A.Ş.", Installments = true },
        //    new Bank { Name = "maximum", BankName = "T.C. İş Bankası", Installments = true },
        //    new Bank { Name = "paraf", BankName = "Halk Bankası", Installments = true },
        //    new Bank { Name = "combo", BankName = "Ziraat Bankası", Installments = true }
        //};
        //}

        //public List<BankRates> SetRatesFromPost(List<BankRates> postedData)
        //{
        //    var banks = GetAvailablePrograms();
        //    var result = new List<BankRates>();
        //    foreach (var bank in banks)
        //    {
        //        var bankRates = new BankRates { Active = 0, Installments = new List<Installment>() };
        //        for (int i = 1; i <= MaxInstallment; i++)
        //        {
        //            var postedInstallment = postedData.Find(rate => rate.Name == bank.Name)?.Installments?.Find(installment => installment.Value == i);
        //            bankRates.Installments.Add(new Installment
        //            {
        //                Value = postedInstallment?.Value ?? 0.0f,
        //                Active = postedInstallment?.Active ?? 0
        //            });
        //        }
        //        result.Add(bankRates);
        //    }
        //    return result;
        //}

        //public List<BankRates> SetRatesDefault()
        //{
        //    var banks = GetAvailablePrograms();
        //    var result = new List<BankRates>();
        //    foreach (var bank in banks)
        //    {
        //        var bankRates = new BankRates { Active = 0, Installments = new List<Installment>() };
        //        for (int i = 1; i <= MaxInstallment; i++)
        //        {
        //            bankRates.Installments.Add(new Installment
        //            {
        //                Value = (float)(1 + i + (i / 5) + 0.1),
        //                Active = bank.Installments ? 1 : 0
        //            });
        //            if (i == 1)
        //            {
        //                bankRates.Installments[i - 1].Value = 0.0f;
        //                bankRates.Installments[i - 1].Active = 1;
        //            }
        //        }
        //        result.Add(bankRates);
        //    }
        //    return result;
        //}

        //public List<BankRates> SetRatesNull()
        //{
        //    var banks = GetAvailablePrograms();
        //    var result = new List<BankRates>();
        //    foreach (var bank in banks)
        //    {
        //        var bankRates = new BankRates { Active = 0, Installments = new List<Installment>() };
        //        for (int i = 1; i <= MaxInstallment; i++)
        //        {
        //            bankRates.Installments.Add(new Installment { Value = 0.0f, Active = 0 });
        //        }
        //        result.Add(bankRates);
        //    }
        //    return result;
        //}

        //public string CreateRatesUpdateForm(List<BankRates> rates)
        //{
        //    string returnHtml = "<table class=\"esnekpos_table table\">" +
        //                        "<thead><tr><th>Banka</th><th>Durum</th>";
        //    for (int i = 1; i <= MaxInstallment; i++)
        //    {
        //        returnHtml += $"<th>{i} taksit</th>";
        //    }
        //    returnHtml += "</tr></thead><tbody>";

        //    var banks = GetAvailablePrograms();
        //    foreach (var bank in banks)
        //    {
        //        var bankRates = rates.Find(rate => rate.Name == bank.Name);
        //        if (bankRates == null)
        //            continue;

        //        returnHtml += "<tr>" +
        //                      $"<th><img src=\"{MaxInstallment}catalog/view/theme/default/image/esnekpos_payment/{bank.Name}.svg\" width=\"105px\"></th>" +
        //                      "<th><select name=\"payment_esnekpos_payment_rates[" + bank.Name + "][active]\">" +
        //                      "<option value=\"1\">Aktif</option>" +
        //                      $"<option value=\"0\" {(int)bankRates.Active == 0 ? "selected=\"selected\"" : ""}>Pasif</option>" +
        //                      "</select></th>";

        //        foreach (var installment in bankRates.Installments)
        //        {
        //            var active = installment.Active;
        //            var value = installment.Value;
        //            returnHtml +=
        //                "<td>" +
        //                $"Aktif <input type=\"checkbox\" name=\"payment_esnekpos_payment_rates[{bank.Name}][installments][{i}][active]\" value=\"1\" {(int)active == 1 ? "checked=\"checked\"" : ""}/>" +
        //                $"% <input type=\"number\" step=\"0.01\" maxlength=\"4\" size=\"4\" style=\"width:60px\" {(int)active == 0 ? "disabled=\"disabled\"" : ""} value=\"{value}\" name=\"payment_esnekpos_payment_rates[{bank.Name}][installments][{i}][value]\"/>" +
        //                "</td>";
        //        }
        //        returnHtml += "</tr>";
        //    }

        //    returnHtml += "</tbody></table>";
        //    return returnHtml;
        //}

        //public List<BankRates> CalculatePrices(float price, List<BankRates> rates)
        //{
        //    var banks = GetAvailablePrograms();
        //    var result = new List<BankRates>();
        //    foreach (var bank in banks)
        //    {
        //        var bankRates = rates.Find(rate => rate.Name == bank.Name);
        //        if (!bank.Installments || bankRates == null)
        //            continue;

        //        var bankResult = new BankRates { Active = bankRates.Active, Installments = new List<Installment>() };
        //        for (int i = 1; i <= MaxInstallment; i++)
        //        {
        //            var installmentValue = bankRates.Installments.Find(installment => installment.Value == i)?.Value ?? 0.0f;
        //            bankResult.Installments.Add(new Installment
        //            {
        //                Value = (100 + installmentValue) * price / 100,
        //                Active = installmentValue
        //            });
        //        }
        //        result.Add(bankResult);
        //    }
        //    return result;
        //}
    }
}