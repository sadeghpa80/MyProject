using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace ArianWebsiteMVC.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly DataContext _db = new DataContext();

        private readonly IProductRepository _productRepository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly IPriceDateRepository _priceDateRepository;

        public ProductsController()
        {
            _productRepository = new ProductRepository(_db);
            _productGroupRepository = new ProductGroupRepository(_db);
            _priceDateRepository = new PriceDateRepository(_db);
        }
        // GET: Products
        public ActionResult Index()
        {
            var products = _db.Products.Include(p => p.ProductGroup);
            return View(products.ToList());
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(_productGroupRepository.GetAllProductGroups(), "GroupId", "GroupName");
            return PartialView();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,ProductWeight,ProductLenght,GroupId,ProductImage,PreOrder,SpecialProduct,ProductPrice,Discount,DiscountPrice,ProductStock")] Product product)
        {
            if (ModelState.IsValid)
            {

                _productRepository.InsertProduct(product);
                _productRepository.Save();
                _priceDateRepository.PriceDateUpdate(1);
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(_productRepository.GetAllProducts(), "GroupId", "GroupName", product.GroupId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _productRepository.GetProductById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(_productGroupRepository.GetAllProductGroups(), "GroupId", "GroupName", product.GroupId);
            return PartialView(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductWeight,ProductLenght,GroupId,ProductImage,PreOrder,SpecialProduct,ProductPrice,Discount,DiscountPrice,ProductStock")] Product product)
        {
            if (ModelState.IsValid)
            {

                _productRepository.UpdateProduct(product);
                _productRepository.Save();
                _priceDateRepository.PriceDateUpdate(1);

                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(_productGroupRepository.GetAllProductGroups(), "GroupId", "GroupName", product.GroupId);
            return View(product);
        }      
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _productRepository.Dispose();
                _productGroupRepository.Dispose();
               _priceDateRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
