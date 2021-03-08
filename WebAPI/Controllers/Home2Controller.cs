using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Repo;
using dtSearch.Engine;
using BAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace WebAPI.Controllers
{

    public class Home2Controller : ApiController
    {
        DtCrud repo;
        public Home2Controller()
        {
            repo = new DtCrud();
        }
        [HttpPost]
        [Route("api/createindex")]
        public IHttpActionResult createindex(IndexModel mod)
        {
            if(mod.path.Contains("fakepath"))
            {
                string[] str = Directory.GetFiles(@"D:\schools");
                mod.path = str[0];
            }
            string t = mod.path;
            int ls = t.LastIndexOf('.');
            int l = t.Length;
            t = t.Remove(ls, l - ls);
            t = t.Replace("\\",".");
            int lss = t.LastIndexOf('.');
            l = t.Length;
            t = t.Remove(lss, l - lss);
            t = t.Replace(".","\\");
            var result = repo.CreateIndex(mod.indexname,t);
            return Ok(result);
        }
        [HttpGet]
        [Route("api/search")]
        public IHttpActionResult Search(string find,string option)
        {
            List<ResultModel> result = new List<ResultModel>();
            result = repo.Search(find,option);
            return Ok(result);
        }

        [HttpDelete]
        [Route("api/deleteindex")]
        public IHttpActionResult DeleteIndex(JObject txt)
        {
            var s = JsonConvert.DeserializeObject<List<IndexModel>>(txt["models"].ToString());
            var res = repo.DeleteIndex(Convert.ToString(s[0].indexname));
            return Ok(res);
        }

        [HttpGet]
        [Route("api/indices")]
        public IHttpActionResult GetIndices()
        {
            List<IndexModel> lst = new List<IndexModel>();
            lst = repo.listindices();
            return Ok(lst);
        }
        //[HttpDelete]
        //[Route("api/deletefile")]
        //public IHttpActionResult DeleteFile(string txt)
        //{
        //    var result = repo.DeleteFile(txt);
        //    return Ok(result);
        //}
        [HttpPut]
        [Route("api/updateindexfile")]
        public IHttpActionResult UpdateIndexFile(IndexModel mod)
        {
            string result = repo.UpdateIndex(mod.indexname, mod.path);
            return Ok(result);
        }
    }
}
