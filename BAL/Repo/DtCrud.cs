using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dtSearch.Engine;
using System.IO;

namespace BAL.Repo
{
   public class DtCrud
    {
        IndexJob indexjob;
        public DtCrud()
        {
            indexjob = new IndexJob();
        }
        public string CreateIndex(string indexname,string path)
        {
            string dir = @"C:\Users\meharr\AppData\Local\dtSearch\"+indexname;
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                return "index with name "+indexname+" already exist";
            }
            indexjob.FoldersToIndex.Add(path+@"\<+>");
            //foreach(var file in Directory.GetFiles(path))
            //{
            //    indexjob.ToAddFileListName = file;
            //}
            indexjob.IndexPath = dir;           
            indexjob.ActionCreate = true;
            indexjob.ActionAdd = true;
            bool result = indexjob.Execute();
            if(result == true)
            {
                return "Index Succesfully created";
            }
            else
            {
                return "Something Wrong";
            }
        }
        public List<ResultModel> Search(string txt,string option)
        {
            List<ResultModel> lst = new List<ResultModel>();

            SearchJob sj1 = new SearchJob();
            sj1.BooleanConditions = txt;
            string[] ddlist;
            ddlist = Directory.GetDirectories(@"C:\Users\meharr\AppData\Local\dtSearch");
            sj1.IndexesToSearch.AddRange(ddlist);
            sj1.MaxFilesToRetrieve = 100;
            if (option == "stemming")
            {
                sj1.SearchFlags = SearchFlags.dtsSearchStemming;
            }
            else if (option == "phonic")
            {
                sj1.SearchFlags = SearchFlags.dtsSearchPhonic;
            }
            else if (option == "fuzzy")
            {
                sj1.Fuzziness = 5;
                sj1.SearchFlags = SearchFlags.dtsSearchFuzzy;
            }
            sj1.Execute();
 
            SearchResults result = sj1.Results;

            for(int i = 0; i<result.Count;i++)
            {
                var tile = HighlightResult(i,result);
                int len = tile.Length;
                int ind = tile.IndexOf("Filename");
                tile = tile.Remove(ind, Math.Abs(ind-len));
                result.GetNthDoc(i);
                SearchResultsItem item = result.CurrentItem;

                ResultModel mod = new ResultModel();
                mod.DisplayName = item.DisplayName;
                mod.FileName = item.Filename;
                mod.Title = tile;
                mod.HitCount = item.HitCount;
                lst.Add(mod);
            }
            return lst;
        }

        public string HighlightResult(int i, SearchResults result)
        {
            FileConverter fc = new FileConverter();
            fc.SetInputItem(result, i);
            fc.OutputFormat = OutputFormats.itUnformattedHTML;
            fc.OutputToString = true;
            fc.BeforeHit = "<b style='color:red'>";
            fc.AfterHit = "</b>";

            //fc.Flags = ConvertFlags.dtsConvertGetFromCache | ConvertFlags.dtsConvertInputIsHtml;
            fc.Execute();
            return fc.OutputString; /*fc.OutputString.Substring(0, fc.OutputString.IndexOf("</dl>") + 5);*/
        }

        public string DeleteIndex(string txt)
        {
            string path = @"C:\Users\meharr\AppData\Local\dtSearch\" + txt;
            if(!Directory.Exists(path))
            {
                return "Index with name " + txt + " Not Exist";
            }
            else
            {
                try
                {
                    //indexjob.IndexPath = path;
                    //indexjob.ActionRemoveListed = true;
                    //indexjob.ActionRemoveDeleted = true;
                    //indexjob.Execute();

                    string[] files = Directory.GetFiles(path);
                    foreach(var file in files)
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(path);
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }
                return "Succesfully Deleted " + txt;
            }
        }

        public List<IndexModel> listindices()
        {
            string[] indices = Directory.GetDirectories(@"C:\Users\meharr\AppData\Local\dtSearch");
            List<IndexModel> lst = new List<IndexModel>();
            foreach(var dir in indices)
            {
                IndexModel ind = new IndexModel();
                int x = dir.Length;
                int y = dir.LastIndexOf("\\");
                ind.indexname = dir.Substring(y+1);
                lst.Add(ind);
            }
            return lst;
        }
    //    public string DeleteFile(string indexname)
    //    {
    //        indexjob.IndexPath = @"C:\Users\meharr\AppData\Local\dtSearch\" + indexname;
    //        indexjob.ActionRemoveDeleted = true;
    //        indexjob.ToRemoveListName = @"D:\Study Material\dtsearch doc\1.txt";
    //        bool success = indexjob.Execute();
    //        if(success != true)
    //        {
    //            return "something Wrong";
    //        }
    //        else
    //        {
    //            return "File deleted";
    //        }
    //    }
    public string UpdateIndex(string indexname,string filepath)
        {
            string dir = @"C:\Users\meharr\AppData\Local\dtSearch\" + indexname;
            if (!Directory.Exists(dir))
            {
                return "index with name " + indexname + " Not exist";
            }
            else
            {
                string indexpath = @"C:\Users\meharr\AppData\Local\dtSearch\" + indexname;
                indexjob.ActionAdd = true;
                indexjob.IndexPath = indexpath;
                indexjob.IndexingFlags = IndexingFlags.dtsIndexCacheText | IndexingFlags.dtsAlwaysAdd | IndexingFlags.dtsIndexCacheTextWithoutFields;
                indexjob.ToAddFileListName = filepath;
                bool txt = indexjob.Execute();
                if(txt!=true)
                {
                    return "Failed to update";
                }
                else
                {
                    return "Updated Succesfully";
                }
            }
        }
    }
}
