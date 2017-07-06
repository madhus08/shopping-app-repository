using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingAppFB.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace ShoppingAppFB.Controllers
{
    public class HomeController : Controller
    {
        //database connectivity
        ShoppingAppFBEntities2 db = new ShoppingAppFBEntities2();
        //HomePage
        public ActionResult Index()
        {
            return View();
        }
        //To sell a product
        public ActionResult SellProduct()
        {
            SellProductDetail selldet = new SellProductDetail();
            return View(selldet);
        }
        //post data from view form to the database
        [HttpPost]
        public ActionResult SellProduct(SellProductDetail sdet,HttpPostedFileBase image)
        {
            try
            {
                var db = new ShoppingAppFBEntities2();
                sdet.ItemImage = new byte[image.ContentLength]; 
                image.InputStream.Read(sdet.ItemImage, 0, image.ContentLength); //convert image to varbinary format in database
                sdet.SellerID = Session["id"].ToString();
                db.SellProductDetails.Add(sdet); //saves data to the database
                db.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            return View(sdet);
        }
        //displays products available for buying
        public ActionResult BuyProduct()
        {
            var db = new ShoppingAppFBEntities2();
            var item = (from d in db.SellProductDetails select d).ToList(); //lists the products available for buying from the database
            return View(item);
        }
        
        //add the selected item to cart table
        public ActionResult AddtoCart(int id)
        {
            SellProductDetail prod = db.SellProductDetails.SingleOrDefault(p => p.ItemID == id);
            var cartItem = db.CartDetails.SingleOrDefault(c => c.ItemID == id);
            if (cartItem == null)
            {
                cartItem = new CartDetail     //saves the selected item to cart table based on item id
                {
                    ItemID = prod.ItemID,
                    Count = 1,
                    SellerID = Session["id"].ToString()
                    //SellerID = "1234"
                };
                db.CartDetails.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
                db.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Viewcart", "Home", new { id = cartItem.ItemID }); //redirects to viewcart and display details of selected products
        }
        //display details of products added to the cart by the user
        public ActionResult Viewcart(int id)
        {
            var model = new List<CartDetail>();
            model = db.CartDetails.Where(a => a.ItemID == id).ToList();
            List<SellProductDetail> li = new List<SellProductDetail>();
            foreach (var Prodc in model)
            {
                SellProductDetail sdet = new SellProductDetail();
                sdet = db.SellProductDetails.SingleOrDefault(m => m.ItemID == Prodc.ItemID);
                li.Add(sdet);
            }
            ViewData["productim"] = li;
            return View(model);
        }
        //removes from cart when remove from cart is clicked
        public ActionResult Remove(int id)
        {

            var cartit = db.CartDetails.SingleOrDefault(c => c.ItemID == id);
            if (cartit != null)
            {
                if (cartit.Count > 1) //decreases the quantity by 1 when there are multiple items 
                {
                    cartit.Count--;
                }
                else
                {
                    db.CartDetails.Remove(cartit); //removes the only item from cart
                }
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Fail";
            }
            return RedirectToAction("Viewcart", "Home", new { id = cartit.ItemID }); 
        }
        //Saves the confirmed items details to the order table
        public ActionResult Checkout(int id, int tot)
        {
            //  var result = (from d in db.CartDetails where d.ItemID == id select d).FirstOrDefault();

            OrderDetail ord = new OrderDetail();
            ord.ItemID = id;
            ord.SellerID = Session["id"].ToString();
            ord.BuyerID = Session["id"].ToString();
            ord.Total = tot.ToString();
            db.OrderDetails.Add(ord);
            db.SaveChanges();
           // return RedirectToAction(
           return RedirectToAction("SellerDetails","Home",new { ItemID1 = ord.ItemID }); //saves the details of item and the buyer in the buyer details

        }
        //saves details to buyer product details table 
        public ActionResult SellerDetails(int ItemID1)
        {
            SellProductDetail prod = db.SellProductDetails.FirstOrDefault(p => p.ItemID == ItemID1);
           // Session["ItemID1"] = prod.ItemID.ToString();
            OrderDetail odet = db.OrderDetails.FirstOrDefault(o => o.ItemID == prod.ItemID);
            var bdet = db.BuyerProductDetails.FirstOrDefault(c => c.ItemID == ItemID1);
            if (bdet == null)
            {
                bdet = new BuyerProductDetail
                {
                    ItemID = odet.ItemID,
                    OrderID = odet.OrderID,
                    ItemName = prod.ItemName,
                    ItemCategory = prod.ItemCategory,
                    ItemDescription = prod.ItemDescription,
                    Total = odet.Total,
                    BuyerID = odet.BuyerID,
                    SellerID = odet.SellerID

                };
                db.BuyerProductDetails.Add(bdet);
                db.SaveChanges();
                Session["itemid"] = bdet.ItemID.ToString();
                ViewBag.orderid = bdet.OrderID;
                
            }
            return RedirectToAction("Payment", "Home"); //redirects to the payment page
          
        }
        //Payment page
        public ActionResult Payment()
        {
            return View();
        }
        //A mail wil be sent upon successful payment from admin to the buyer with the ordered product details
        public ActionResult Success()
        {
            string id = Session["itemid"].ToString();
            
            SmtpClient server = new SmtpClient("smtp.gmail.com");
            server.Port = 587;
            server.UseDefaultCredentials = false;
            server.Credentials = new NetworkCredential("smadhu0893@gmail.com", "madhu@201103045");
            server.EnableSsl = true;
            MailMessage email = new MailMessage();
            email.From = new MailAddress("smadhu0893@gmail.com");
            var toaddress = Session["email"].ToString();
            email.To.Add(toaddress);
            List<BuyerProductDetail> bdet = (from st in db.BuyerProductDetails where st.ItemID.ToString()==id select st).ToList();
            email.Subject = "Order Placed";
            foreach (var item in bdet) //ordered items will be retrieved from the table and sent in mail
            {
                string Body = "Hi ," + "\n" + "Your order has been placed and the details are given below: " + "\n" +"Order ID:"+"\t"+item.OrderID+"\t" +"Item ID: "+"\t"+item.ItemID +"\t" +"Item Name: " +"\t"+ item.ItemName +"\n"+ "Category:" +"\t"+ item.ItemCategory+ "\t"+"Description:"+"\t"+item.ItemDescription+"\t"+"Total:"+ "\t"+item.Total + " ";
                email.Body = Body;
            }
            try
            {
                server.Send(email);
            }
            catch (SmtpFailedRecipientException error)
            {
                return View(error.FailedRecipient);
            }
            return RedirectToAction("RemovefromSell","Home",new { id = Session["itemid"] }); 
        }
        //removes the ordered item from the seller product details
        public ActionResult RemovefromSell(int id)
        {
            var sell = db.SellProductDetails.SingleOrDefault(c => c.ItemID == id);
            if (sell != null)
            {
                db.SellProductDetails.Remove(sell);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Fail";
            }
           // return RedirectToAction("Viewcart", "Home", new { id = cartit.ItemID }); ;
            return View("Success");
        }

    }




    }