﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Prj_Dh_Food_Shop.Controllers
{
    public class CategoriesController : Controller
    {
        private Entity_Dh_Food db = new Entity_Dh_Food();
        public ActionResult Index(Search_Categories model)
        {
            model.txbName = model.txbName == null ? string.Empty : model.txbName.Trim();

            model.page = model.page == 0 ? 1 : model.page;
            model.pageSize = model.pageSize == 0 ? 3 : model.pageSize;

            var data = from c in db.Categories
                       where (string.IsNullOrEmpty(model.txbName) || c.name.Contains(model.txbName))
                       select new Search_Categories()
                       {
                           id = c.id,
                           name = c.name,
                           create_date = c.create_date,
                       };

            var rs = data.OrderBy(x => x.id).Skip(((model.page - 1) * model.pageSize)).Take(model.pageSize).ToList() ?? new List<Search_Categories>();
            model.lstData = rs;
            model.totalRecord = data.Count();
            model.totalPage = (int)Math.Ceiling((decimal)model.totalRecord / model.pageSize);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories model)
        {
            db.Categories.Add(model);
            var msg = "";
            var status = 0;
            model.create_date = DateTime.Now;
            db.SaveChanges();
            msg = "Thêm mới loại sản phẩm thành công!";
            status = 1;
            return Json(new { msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories cates = db.Categories.Find(id);
            if (cates == null)
            {
                return HttpNotFound();
            }
            return PartialView("PartialEdit", cates);
        }

        [HttpPost]
        public ActionResult Edit(Categories cates)
        {
            var msg = "";
            var status = 0;
            var result = db.Categories.SingleOrDefault(b => b.id == cates.id);

            result.name = cates.name;

            db.SaveChanges();
            msg = "Cập nhật thông tin loại sản phẩm thành công!";
            status = 1;
            return Json(new { msg = msg, status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories cates = db.Categories.Find(id);
            if (cates == null)
            {
                return HttpNotFound();
            }
            return PartialView("PartialDelete", cates);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Products pro = db.Products.Where(x => x.id_category == id).FirstOrDefault();
            Orders_detail ord = db.Orders_detail.Where(x => x.id_product == id).FirstOrDefault();
            var msgDel = "";
            var status = 0;
            if (pro != null)
            {
                msgDel = "Loại sản phẩm có sản phẩm đang kích hoạt. Không thể xóa!";
                status = -1;
            }
            else if (ord != null)
            {
                msgDel = "Loại sản phẩm có sản phẩm đang hoặc đã có trong đơn đặt hàng. Không thể xóa!";
                status = -1;
            }
            else
            {
                Categories cates = db.Categories.Find(id);
                db.Categories.Remove(cates);
                db.SaveChanges();
                msgDel = "Xóa loại sản phẩm thành công!";
                status = 1;
            }

            return Json(new { msg = msgDel, status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}