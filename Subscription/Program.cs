using System;
using System.Collections.Generic;
using System.Linq;

namespace Subscription
{
    class BillingHelper
    {
        static void Main(string[] args)
        {
            //  Console.WriteLine("Hello World!");
              Subscription subObj = new Subscription();
            subObj.Start = new DateTime(2021, 01, 12);
            subObj.End = null;
            subObj.PricePerPeriod = 2;
            DateTime billingEnd = new DateTime(2017, 05, 03);

            //calling bill subscription
            subObj.BillSubscription(subObj, billingEnd);
            
            var subscription = new Subscription { Start = new DateTime(2017, 03, 03), End = null, PricePerPeriod = 10.00m };
            var discounts = new List<Discount>
            {
                   new Discount { Start = new DateTime(2017, 03, 03), End = new DateTime(2017, 03, 17), PercentReduction = 50},
                new Discount { Start = new DateTime(2017, 03, 10, 12, 0, 0), End = new DateTime(2017, 04, 10, 12, 0, 0), PercentReduction = 20},
             
            };
         // calling billsubscriptionwith discount
           var invoiceLines = subObj.BillSubscriptionWithDiscounts(subscription, discounts, billingEnd);

            foreach(InvoiceLine invoice in invoiceLines)
            {
                Console.Write("Start Date: " + invoice.Start + "|");
                Console.Write("End Date: " + invoice.End + "|"); 
                Console.Write("Price per Period: " + invoice.PricePerPeriod + "|");
                Console.Write("Duration: " + invoice.Duration + "|");
                Console.Write("Total: " + invoice.Total + "|");
                Console.WriteLine();
               
            }


        }


    }

    public class Subscription
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public decimal PricePerPeriod { get; set; }

        public InvoiceLine BillSubscription(Subscription subscription, DateTime billingEnd)
        {
            InvoiceLine invoiceObj = new InvoiceLine();
            if (subscription != null)
            {
                DateTime checkSubcStartDate = subscription.Start;
                ///if (checkSubcStartDate == DateTime.Now)
                {
                    invoiceObj.Start = subscription.Start; //month start
                    if(subscription.End == null)
                    {
                        invoiceObj.End = billingEnd;
                    }
                    else
                    {
                        invoiceObj.End = Convert.ToDateTime(subscription.End); //month end day
                    }
                   
                    invoiceObj.Duration =Convert.ToDecimal( (invoiceObj.End.Date- subscription.Start.Date).TotalDays);
                    invoiceObj.PricePerPeriod = subscription.PricePerPeriod;
                    invoiceObj.Total = invoiceObj.Duration * invoiceObj.PricePerPeriod; 
                }

            }
            else
            {
                invoiceObj = null;
            }
            return invoiceObj;
        }

        public IEnumerable<InvoiceLine> BillSubscriptionWithDiscounts(Subscription subscription, List<Discount> discounts, DateTime billingEnd)
        {
            List<InvoiceLine> listInvoiceObj = new List<InvoiceLine>();
            DateTime? billedDate = null;
            //Discount disObj = new Discount();
            
            if (subscription!=null)
            {
               
                decimal maxDiscount = discounts.Max(x => x.PercentReduction);
                decimal discountAfter = 0;
                int length = discounts.Count;
               
                // add value of invoice for the subscription when there is no discount
                if(discounts.FirstOrDefault().Start > subscription.Start)
                {
                    InvoiceLine inObj = new InvoiceLine();
                    inObj.Start = subscription.Start;
                    inObj.End = discounts.FirstOrDefault().Start;
                    inObj.Duration = Convert.ToDecimal((inObj.End.Date - inObj.Start.Date).TotalDays);
                    inObj.PricePerPeriod = subscription.PricePerPeriod;
                    inObj.Total = inObj.Duration * inObj.PricePerPeriod;
                    listInvoiceObj.Add(inObj);

                }
                // iterate the no. of  dicounts
               foreach(Discount dis in discounts)
                {
                    InvoiceLine invoiceObj = new InvoiceLine();

                    if (dis.Start >= subscription.Start && dis.End < billingEnd)
                    {
                        
                        if(billedDate != null)
                        {
                            invoiceObj.Start = (DateTime)billedDate;
                            
                        }
                        else
                        {
                            invoiceObj.Start = subscription.Start;
                        }
                        //if(billedDate > dis.Start)
                        //{
                        //    invoiceObj.End = dis.Start;
                        //}
                        //else
                        //{
                            invoiceObj.End = dis.End;
                        //}
                        
                        invoiceObj.Duration = Convert.ToDecimal(( invoiceObj.End - invoiceObj.Start).TotalDays);
                        // condition checks highest discount and add value.
                        if(discountAfter == 0)
                        {
                            invoiceObj.PricePerPeriod = subscription.PricePerPeriod - ((subscription.PricePerPeriod * dis.PercentReduction) / 100);
                        }
                        else
                        {
                            invoiceObj.PricePerPeriod = subscription.PricePerPeriod - ((subscription.PricePerPeriod * discountAfter) / 100);
                        }
                        //invoiceObj.PricePerPeriod = subscription.PricePerPeriod -((subscription.PricePerPeriod * dis.PercentReduction)/ 100);
                        invoiceObj.Total = invoiceObj.Duration * invoiceObj.PricePerPeriod;
                        billedDate = dis.End;
                        //if( billedDate != null && billedDate > dis.Start)
                        //{
                        //    discountAfter = dis.PercentReduction;
                        //}
                        
                        listInvoiceObj.Add(invoiceObj);

                    }
                   
                }

               if(billedDate < billingEnd)
                {
                    InvoiceLine invoiceObj = new InvoiceLine();
                    invoiceObj.Start = (DateTime)billedDate;
                    invoiceObj.End = billingEnd;
                    invoiceObj.Duration = Convert.ToDecimal((invoiceObj.End - invoiceObj.Start).TotalDays);
                    invoiceObj.PricePerPeriod = subscription.PricePerPeriod ;
                    invoiceObj.Total = invoiceObj.Duration * invoiceObj.PricePerPeriod;
                   
                    listInvoiceObj.Add(invoiceObj);
                }

           }
            else
            {
                //if subscription is null
                //listInvoiceObj = null;

            }
                return listInvoiceObj.AsEnumerable();
            
        }
    }
    public class InvoiceLine
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public decimal PricePerPeriod { get; set; }
        public decimal Duration { get; set; } // in periods
        public decimal Total { get; set; }

        
    }

    public class Discount
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public decimal PercentReduction { get; set; }
    }

}

