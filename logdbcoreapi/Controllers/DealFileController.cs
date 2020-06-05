using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace logdbcoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealFileController : ControllerBase
    {
        private readonly IWebHostEnvironment ienv;
        /// <summary>
        /// 图片保存路径
        /// </summary>
        private readonly string basepath = Directory.GetCurrentDirectory()+@"\images\";
        public DealFileController(IWebHostEnvironment env)
        {
            ienv = env;     
        }
        /// <summary>
        /// application/octet-stream
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  FileStreamResult Get(string fname)
        {

            FileStream stream = new FileStream(basepath + @"swiperimgs\"+ fname, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", fname);

        }
        

        /// <summary>
        /// 取全部Post过来的所有文件。
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public IActionResult Post()
        //{
        //    var req = Request.Form; 
        //    var uid = req["uid"].ToString();
        //    var uxm = req["uxm"].ToString();

        //var pa = Path.Combine(ienv.WebRootPath, @"dnFile");//注入的环境变量,取路径
        //    var files = req.Files;
        //    long size = files.Sum(f => f.Length);
        //    return Ok(new { fcount = files.Count, fsize = size, uid, uxm});
        //}

        [HttpPost]
        public int Post([FromForm] IFormCollection collection)
        {
            int retvalue = 0;//成功写入的文件个数。
            var uid = collection["userid"].ToString();
            var uname = collection["username"].ToString();
            var imgpath = collection["imgpath"].ToString();
            var files = collection.Files;
            try 
            {
                foreach (var file in files)
                {
                    if (file.Length <=0)
                    {
                        continue;
                    }
                    var fname = file.FileName;
              
                    var path = Path.Combine(basepath, imgpath + @"/"+fname);

                    using (FileStream fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        retvalue++;
                    }

                }
            }
            catch 
            {
                retvalue = 0;
            }

            return retvalue;
        }
    }
}