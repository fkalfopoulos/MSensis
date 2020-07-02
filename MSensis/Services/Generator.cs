using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSensis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WkWrap.Core;

namespace MSensis.Services
{
    public class Generator
    {
        private readonly MSensisContext _db;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Generator> _logger;

        public Generator(MSensisContext db, IHostingEnvironment  env)
        {
            _db = db;
            _env = env;
        }



        public async Task<IActionResult> CreatePDF(string id)
        {


            Pdf pdf = _db.Pdfs.Include(u => u.Invoice)
               .Include(u => u.Client)
               .Include(u => u.Company)
               .Where(c => c.Id == id).SingleOrDefault();

            List<Product> productList = await _db.Products.Where(a => a.PdfId == pdf.Id).ToListAsync();
            var cssString = @"<style>
                .invoice-box {
                    max-width: 2480px;
                    max-height: 3495px:
                    width: 100%;
                    height: 90%;
                    margin: auto 0;
                    padding: 30px 7px;
                    font-size: 15px;
                    line-height: 24px;
                    font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;
                    color: #555;
                    letter-spacing: 0px;
                }
                .invoice-box table {
                    width: 100%;
                    line-height: inherit;
                    text-align: left;
                }
                .invoice-box table td {
                    padding: 5px 0px;
                    vertical-align: top;
                }
                .invoice-box table tr.top table td {
                    padding-bottom: 50px;
                }
                .invoice-box table .top td .title {
                    font-size: 32px;
                    line-height: 32px;
                    padding-left: 65px !important;
                    color: #333;
                    float: left;
                    width: 80%;
                    text-align: left;
                }
                .invoice-box table .top td .logo {
                    padding-left: 0px !important;
                    float: left;
                    max-width: 65%;
                    height: auto;
                    text-align: left;
                }
                .invoice-box table .top .company {
                    float: right;
                    font-size: 13.5px;
                    text-align: left !important;
                    width: 30%;
                    align: right !important;
                    margin-right: 0;
                    padding-right: 10px;
                    margin-left: 10px;
                }
                .invoice-box table .top .company hr{
                    margin-left: auto;
                    margin-right: auto;
                    width: 100%;
                    height: 2px;
                    margin-top: 2px;
                    margin-bottom: 0;
                    padding-bottom: 1px;
                }
                .invoice-box .debtor-table{
                    margin-left: 55px !important;
                    margin-bottom: 45px !important;
                }
                .invoice-box .debtor-table .debtor-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 0 0 0 15px;
                }
                .invoice-box .debtor-table .debtor-info tr{
                    padding: 0 0 0 0;
                    margin: 0 0 0 0;
                }
                .invoice-box .debtor-table .debtor-name,
                .invoice-box .debtor-table .debtor-address,
                .invoice-box .debtor-table .debtor-city{
                    font-size: 16px;
                    text-align: left !important;
                    padding-bottom: 0px;
                    line-height: normal;
                }
                .invoice-box .invoice-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 8px 0 65px 55px !important;
                    width: 30%;
                }
                .invoice-box .invoice-info tr{
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0;
                    line-height: 64%;
                    font-size: 16px;
                    border-collapse: collapse;
                    border-spacing; 0;
                }
                .invoice-box .invoice-info tr td:nth-child(1){
                    width: 40%;
                }
                .invoice-box .invoice-info tr td:nth-child(2){
                    width: 60%
                    padding-left: 2px;
                    text-align: left;
                    align: left;
                }
                .invoice-box .item-table tr.heading td {
                    background: #eee;
                    border-bottom: 1px solid #ddd;
                    font-weight: bold;
                    font-size: 15px;
                }
                .invoice-box .item-table tr.details td {
                    padding-bottom: 20px;
                }
                .invoice-box .item-table{
                    margin-bottom: 25px !important;
                }
                .invoice-box .item-table tr.item td {
                    border-bottom: 1px solid #eee;
                    font-size: 14px;
                }
                .invoice-box .item-table tr.item.last td {
                    border-bottom: none;
                }
                .invoice-box .item-table tr td:nth-child(1){
                    border-top: 2px solid #eee;
                    width: 26%;
                    padding-left: 4px;
                }
                .invoice-box .item-table tr td:nth-child(2){
                    width: 34%;
                }
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5),
                .invoice-box .item-table tr td:nth-child(6){
                    text-align: center;
                }
                .invoice-box .item-table tr td:nth-child(3){
                    width: 12%;
                    text-align: center;
                }
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5){
                    width: 8%;
                }
                .invoice-box .item-table tr td:nth-child(6){
                    width: 12%
                }
                .invoice-box .total-table{
                    border-collapse: collapse;
                    border-spacing; 0;
                    width: 18%;
                    float: right;
                    margin: 5px 35px 0 0 !important;
                    padding: 0 0 0 0 !important;
                }
                .invoice-box .total-table .total{
                    border-top: 1px solid #888;
                }
                .invoice-box .total-table tr td:nth-child(1){
                    font-weight: 600;
                    text-align: right;
                    font-size: 15px;
                    padding-bottom: 1px;
                    width: 40%;
                }
                .invoice-box .total-table tr td:nth-child(2){
                    font-weight: 800;
                    text-align: left;
                    font-size: 15px;
                    padding-bottom: 1px;
                    padding-left: 6px;
                    width: 60%;
                }
                .invoice-box .disclaimer-table{
                    width: 100%;
                    border-top: 2px solid #DDD;
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0 !important;
                    position: fixed !important;
                    bottom: 4%;
                    left: 0%;
                }
                .invoice-box .disclaimer-table .message .message-text{
                    font-size: 16px;
                    text-align: center;
                    padding: 5px 25px !important;
                }
                @media only screen and (max-width: 600px) {
                    .invoice-box table tr.top table td {
                        width: 100%;
                        display: block;
                        text-align: center;
                    }
                    .invoice-box table tr.information table td {
                        width: 100%;
                        display: block;
                        text-align: center;
                    }
                }
            </style>";

            //Company string
            string companyString = @"<div class=invoice-box>
                <table cellpadding=0 cellspacing=0>
                <tr class=top>
                <td colspan=2>
                <table>
                <tr>
                <td class=title>";

           
                companyString += "<img class=logo src=" + pdf.Company.ImageSrc + " />";
            
               
            

            companyString +=
                "</td>"
                + "<td class=company>"
                + "<b>" + pdf.Company.Name + "</b>"
                + "<hr />"
                + pdf.Company.Address
                + "<br />" +  pdf.Company.City
                + "<br />"
                + "<b>Tel: </b>" + pdf.Company.TelephoneNumber
                + "<br />"
                
                + "</tr> "
                + "</table> "
                + "</td>"
                + "</tr>"
                + "</table>";

            string debtorString = "";

            //Debtor string
            if (pdf.Client != null)
            {
                debtorString = @"<table class=debtor-table cellpadding=0 cellspacing=0>"
                    + "<tr>"
                        + "<td class=debtor-name>"
                            + pdf.Client.CompanyName    
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td class=debtor-address>"
                            + pdf.Client.Address
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td class=debtor-city>"
                            + pdf.Client.Email + " " + pdf.Client.City
                        + "</td>"
                    + "</tr>"
                + "</table>";
            }
        
            //Spacer string
            string spacerString = @"<br />";

            //Invoice info
            string invoiceString = @"<table class=invoice-info cellpadding=0 cellspacing=0>"
                + "<tr>"
                + "<td><b>Date: </b></td>"
                + "<td>" + pdf.DateTimeString + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td><b>Invoice No: </b></td>"
                //+ "<td>" + invoiceNumber + "</td>"
                + "</tr>"
                + "</table>";

            //Product string
            string productString = @"<table class=item-table cellpadding=0 cellspacing=0>"
                + "<tr class=heading> "
                + "<td>#</td>"
                + "<td>Description</td>"
                + "<td>Price</td>"
                + "<td>Qnt</td>"
                + "<td>VAT</td>"
                + "<td>Total</td>"
                + "</tr>";

            //Table string
            string tableString = "";
            decimal totalBeforeDiscount = 0;

            decimal subTotalAmount = 0;
            decimal vatTotalAmount = 0;
            decimal discount = 0;
            int discountPercentage = pdf.Invoice.Discount;

            for (int i = 0; i < productList.Count; i++)
            { 
                    Product product = productList[i];
                    decimal total = (decimal)(product.PricePerUnit * product.Quantity);
                    decimal subTotal = (decimal)(product.PricePerUnit * 100) / product.Vat;
                    totalBeforeDiscount += total;
                    subTotalAmount += subTotal;
                
                vatTotalAmount = totalBeforeDiscount;
                discount = (vatTotalAmount * discountPercentage) / 100;
               

                tableString += @"<tr class=item>"
                        + "<td>" + product.Code + "</td>"
                        + "<td>" + product.Name + "</td>"
                        + "<td>" + product.Quantity + "</td>"
                        + "<td>&euro; " + String.Format("{0:N2}", product.PricePerUnit) + "</td>" 
                        + "<td>" + String.Format("{0}%", product.Vat) + "</td>" 
                        + "</tr>";
            }

            var sumvat = vatTotalAmount / 100 * 24;
            var sum = sumvat + vatTotalAmount;
            //Total string
            string totalString = @"<table class=total-table cellspacing=0 cellpadding=0>"
                + "<tr class=vat>"
                    + "<td>Price without taxes:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", vatTotalAmount.ToString("#.##")) + "</td>"
                + "</tr>"
                + "<tr class=subtotal>"
                    + "<td>Total Discount:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", discount) + "</td>"
                + "</tr>"
                + "<tr class=discount>"
                    + "<td>Total Vat:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", sumvat) + "</td>"
                + "</tr>"
                + "<tr class=total>"
                    + "<td>Grant Total:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", sum) + "</td>"
                + "</tr>"
                + "</table>";



            //Full HTML string
            string htmlContent = cssString + companyString + debtorString + spacerString
                + invoiceString + productString + tableString + totalString;

            string save_path = _env.WebRootPath + "/wkhtmltopdf/bin/libwkhtmltox.so";

             

            var wkhtmltopdf = new FileInfo(save_path);
            var converter = new HtmlToPdfConverter(wkhtmltopdf);
            var pdfBytes = converter.ConvertToPdf(htmlContent);

            FileResult fileResult = new FileContentResult(pdfBytes, "application/pdf");
            fileResult.FileDownloadName = "invoice-" + id + ".pdf";
            return fileResult;
        }

    }


    public static class Pages
    {
        public static class Dashbaordv1
        {
            public const string Url = "/Adminlte/Dashboardv1";
        }
        public static class Dashbaordv2
        {
            public const string Url = "/Adminlte/Dashboardv2";
        }

        public static class LayoutTop
        {
            public const string Url = "/Adminlte/Top";
        }

        public static class LayoutBoxed
        {
            public const string Url = "/Adminlte/Boxed";
        }

        public static class LayoutFixed
        {
            public const string Url = "/Adminlte/Fixed";
        }

        public static class LayoutCollapsed
        {
            public const string Url = "/Adminlte/Collapsed";
        }

        public static class WidgetIndex
        {
            public const string Url = "/Adminlte/Widget";
        }

        public static class ChartChartJS
        {
            public const string Url = "/Adminlte/ChartJS";
        }

        public static class ChartMorris
        {
            public const string Url = "/Adminlte/Morris";
        }

        public static class ChartFlot
        {
            public const string Url = "/Adminlte/Flot";
        }

        public static class ChartInline
        {
            public const string Url = "/Adminlte/Inline";
        }

        public static class UIElementGeneral
        {
            public const string Url = "/Adminlte/General";
        }

        public static class UIElementIcon
        {
            public const string Url = "/Adminlte/Icon";
        }

        public static class UIElementButton
        {
            public const string Url = "/Adminlte/Button";
        }

        public static class UIElementSlider
        {
            public const string Url = "/Adminlte/Slider";
        }

        public static class UIElementTimeline
        {
            public const string Url = "/Adminlte/Timeline";
        }

        public static class UIElementModal
        {
            public const string Url = "/Adminlte/Modal";
        }

        public static class FormGeneral
        {
            public const string Url = "/Adminlte/Form";
        }

        public static class FormAdvanced
        {
            public const string Url = "/Adminlte/Advanced";
        }

        public static class FormEditor
        {
            public const string Url = "/Adminlte/Editor";
        }

        public static class TableSimple
        {
            public const string Url = "/Adminlte/Simple";
        }

        public static class TableData
        {
            public const string Url = "/Adminlte/Data";
        }

        public static class CalendarIndex
        {
            public const string Url = "/Adminlte/Calendar";
        }

        public static class MailboxIndex
        {
            public const string Url = "/Adminlte/Index";
        }

        public static class MailboxCompose
        {
            public const string Url = "/Adminlte/Compose";
        }

        public static class MailboxRead
        {
            public const string Url = "/Adminlte/Read";
        }

        public static class ExampleInvoice
        {
            public const string Url = "/Adminlte/Invoice";
        }

        public static class ExampleInvoicePrint
        {
            public const string Url = "/Adminlte/InvoicePrint";
        }

        public static class ExampleProfile
        {
            public const string Url = "/Adminlte/Profile";
        }

        public static class ExampleLogin
        {
            public const string Url = "/Adminlte/Login";
        }

        public static class ExampleRegister
        {
            public const string Url = "/Adminlte/Register";
        }

        public static class ExampleLockscreen
        {
            public const string Url = "/Adminlte/Lockscreen";
        }

        public static class ExampleError404
        {
            public const string Url = "/Adminlte/Error404";
        }

        public static class ExampleError500
        {
            public const string Url = "/Adminlte/Error500";
        }

        public static class ExampleBlankPage
        {
            public const string Url = "/Adminlte/BlankPage";
        }

        public static class ExamplePacePage
        {
            public const string Url = "/Adminlte/PacePage";
        }
    }
    public static class HtmlHelpers
    {

        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            bool result = (controller == currentController && action == currentAction);



            return result ?
                cssClass : String.Empty;
        }

        public static string IsSelected(this IHtmlHelper html, string actions = null)
        {
            var cssClass = "active";


            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string[] actionarr = !String.IsNullOrEmpty(actions) ? actions.Split(',') : new string[] { };


            bool result = (actionarr.Contains<string>(currentAction));



            return result ?
                cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

    }
}

 
 
