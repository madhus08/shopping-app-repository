//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using ShoppingAppFB.Models;
//using System.Web.UI;
//using System.Net;
//using System.Threading.Tasks;
//using System.Net.Mail;

//namespace ShoppingAppFB.Controllers
//{
//    public class AddtoCartController : Controller
//    {
//      //  private UserDataContext _dataContext;
//        public const string CartSessionKey = "ShoppingCartId";
//        ShoppingAppFBEntities ent = new ShoppingAppFBEntities();
//        int ShoppingCartId { get; set; }
//        public static string recatid = null;

//        //public ActionResult AddtoCart(int id)
//        //{
//        //    SellerProductDetail det = ent.SellerProductDetails.SingleOrDefault(p => p.ItemID == id);
//        //    CartDetail det1 = new CartDetail();
//        //    ShoppingCartId = det1.CartID;
//        //  //  int id1 = det1.ItemID;
//        //    var cartItem = ent.CartDetails.SingleOrDefault(c=>c.CartID == ShoppingCartId && c.ItemID==id);
//        //    //recatid = Convert.ToString(prod.CatID);
//        //    if (cartItem == null)
//        //    {
//        //        // Create a new cart item if no cart item exists
//        //        cartItem = new CartDetail
//        //        {
//        //            ItemName = det1.ItemName,
//        //            ItemDescription=det1.ItemDescription,
//        //            ItemCategory=det1.ItemCategory,
//        //            ItemPrice= det1.ItemPrice

//        //        };
//        //       // cartItem.RecordId = storeDB.Carts.Count() + Convert.ToInt32(1);
//        //        ent.CartDetails.Add(cartItem);
//        //        //storeDB.SaveChanges();
//        //    }
//        //    else
//        //    {
//        //        return View("error");
//        //    }
//        //    ent.SaveChanges();
//        //    return RedirectToAction("Viewcart", "Shoppingcart");
//        //}

//        //GET: AddtoCart
//        public ActionResult AddtoCart(int id, string name, string price, string cat, string desc)
//        {
//            var db = new ShoppingAppFBEntities();
//            CartDetail det = new CartDetail();
//            var results = (from c in db.CartDetails
//                           where c.ItemID == id
//                           select c).SingleOrDefault();
//            if (results == null)
//            {
//                det.ItemID = id;
//                det.ItemName = name;
//                det.ItemPrice = price;
//                det.ItemCategory = cat;
//                det.ItemDescription = desc;
//                det.UserID = Session["id"].ToString();
//                det.SellerID = Session["SellerID"].ToString();
//                db.CartDetails.Add(det);
//                db.SaveChanges();
//                // return View("AddtoCart",det);
//            }
//            else
//            {
//                return View("error");

//            }
//            Session["ProdID"] = det.ItemID.ToString();
//            return RedirectToAction("Viewcart", "AddtoCart");
//            // return View("Viewcart");
//        }
//        public ActionResult Viewcart()
//        {
            
//            var model = new List<CartDetail>();
//            model = ent.CartDetails.ToList();
//            List<SellerProductDetail> li = new List<SellerProductDetail>();
//            foreach (var Prodc in model)
//            {
//                SellerProductDetail sa = new SellerProductDetail();
//                sa = ent.SellerProductDetails.SingleOrDefault(m => m.ItemID == Prodc.ItemID);
//                li.Add(sa);
//            }
//            ViewData["productim"] = li;
//            ViewData["path"] = recatid;
//            return View(model);
//        }
//        public ActionResult Remove(int id)
//        {
//            if (id.ToString() == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            CartDetail cartDetail = ent.CartDetails.Find(id);
//            if (cartDetail == null)
//            {
//                return HttpNotFound();
//            }
//            else
//            {
//                ent.CartDetails.Remove(cartDetail);
//                ent.SaveChanges();
//                return RedirectToAction("Index","Home");
//            }
//           // return View("Remove");
//        }
//        public void EmptyCart()
//        {
//            var ProdId = ((int)Session["ProdID"]);
//            var cartItems = ent.CartDetails.Where(
//               cart => cart.ItemID == ProdId);

//            foreach (var cartItem in cartItems)
//            {
//                ent.CartDetails.Remove(cartItem);
//            }
//            // Save changes
//            ent.SaveChanges();
//            //if (Session["[ProdID"].ToString() == null)
//            //{
//            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            //}
//            //CartDetail cartDetail = ent.CartDetails.Find(id);
//            //if (cartDetail == null)
//            //{
//            //    return HttpNotFound();
//            //}
//            //else
//            //{
//            //ent.CartDetails.Remove(cartDetail);
//            // ent.SaveChanges();
//            Response.Redirect(Url.Action("ViewCart", "ViewCart"));
//        }
//        public ActionResult Checkout(int id, string sum)
//        {
//            var result = (from d in ent.CartDetails where d.ItemID == id select d).FirstOrDefault();

//            Order ord = new Order();
//            ord.ItemID = id;
//            ord.SellerID = Session["SellerID"].ToString();
//            ord.UserID = Session["id"].ToString();
//            ord.Total = sum.ToString();
//            ent.Orders.Add(ord);
//            ent.SaveChanges();
//            return View("Success");
//        }
//            //if (id.ToString() == null)
//            //{
//            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            //}
//            ////CartDetail cartDetail = ent.CartDetails.Find(id);
//            ////if (cartDetail == null)
//            ////{
//            ////    return HttpNotFound();
//            ////}
//            ////else
//            ////{
//            ////    ent.CartDetails.Remove(cartDetail);
//            ////    ent.SaveChanges();
//            ////    return View("Success");
//            ////    //return RedirectToAction("Index", "Home");
//            ////}


        
//        ////[HttpPost]
//        ////[ValidateAntiForgeryToken]
//        ////public async Task<ActionResult> Checkout(CartDetail model,UserDetail model1)
//        ////{
//        ////    if (ModelState.IsValid)
//        ////    {
//        ////        var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
//        ////        var message = new MailMessage();
//        ////        message.To.Add(new MailAddress("recipient@gmail.com"));  // replace with valid value 
//        ////        message.From = new MailAddress("sender@outlook.com");  // replace with valid value
//        ////        message.Subject = "Your email subject";
//        ////   //     message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
//        ////        message.IsBodyHtml = true;

//        ////        using (var smtp = new SmtpClient())
//        ////        {
//        ////            var credential = new NetworkCredential
//        ////            {
//        ////                UserName = "user@outlook.com",  // replace with valid value
//        ////                Password = "password"  // replace with valid value
//        ////            };
//        ////            smtp.Credentials = credential;
//        ////            smtp.Host = "smtp-mail.outlook.com";
//        ////            smtp.Port = 587;
//        ////            smtp.EnableSsl = true;
//        ////            await smtp.SendMailAsync(message);
//        ////            return RedirectToAction("Sent");
//        ////        }
//        ////    }
//        ////    return View(model);
//        ////}
//        //////[HttpPost, ActionName("Delete")]
//        //////[ValidateAntiForgeryToken]
//        //////public ActionResult DeleteConfirmed(int id)
//        //////{
//        //////    CartDetail cartDetail = ent.CartDetails.Find(id);
//        //////    ent.CartDetails.Remove(cartDetail);
//        //////    ent.SaveChanges();
//        //////    return RedirectToAction("Index");
//        //////}



//    }
//}