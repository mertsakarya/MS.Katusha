using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace MS.Katusha.Web.Helpers
{
    internal class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files)
        {
            if (context == null)
                throw new ArgumentNullException("context");


            if (files == null)
                throw new ArgumentNullException("files");




            return files;
        }
    }

    public static class BundleHelper
    {
        public static void RegisterBundles() {
            var bundles = new [] {
                                           new Bundle("~/Scripts/KatushaJs", new NoTransform("text/javascript")) {Orderer = new AsIsBundleOrderer() },
                                           new Bundle("~/Scripts/KatushaDebugJs", new NoTransform("text/javascript")) {Orderer = new AsIsBundleOrderer() },
                                           new Bundle("~/Content/KatushaCss", new NoTransform("text/css")) {Orderer = new AsIsBundleOrderer() },
                                           new Bundle("~/Content/KatushaDebugCss", new NoTransform("text/css")) {Orderer = new AsIsBundleOrderer() }
            };
            using(var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/ConfigData/bundles.txt"))) {
                var line = 0;
                while (!reader.EndOfStream) {
                    var text = reader.ReadLine();
                    line++;
                    if (string.IsNullOrEmpty(text)) continue;
                    if(text[0] == '#') continue;
                    var items = text.Split('\t');
                    if(items.Length != 4)
                        throw new Exception("bundles.txt ERROR at line " + line);
                    var contentType = items[0].ToLowerInvariant()[0]; // js / css
                    var where = items[1].ToLowerInvariant()[0]; // all / debug / runtime
                    var locationType = items[2].ToLowerInvariant()[0]; // file / directory / recursivedirectory
                    var path = items[3];
                    IList<Bundle> bundleList = new List<Bundle>();
                    if (contentType == 'j' && (where == 'r' || where == 'a')) { bundleList.Add(bundles[0]); if (where == 'a') bundleList.Add(bundles[1]); }
                    if (contentType == 'j' && where == 'd') { bundleList.Add(bundles[1]); }
                    if (contentType == 'c' && (where == 'r' || where == 'a')) { bundleList.Add(bundles[2]); if (where == 'a') bundleList.Add(bundles[3]); }
                    if (contentType == 'c' && where == 'd') { bundleList.Add(bundles[3]); }
                    if (bundleList.Count == 0) continue;
                    AddContent(path, contentType, locationType, bundleList);
                }
            }
            foreach (var bundle in bundles) {
                BundleTable.Bundles.Add(bundle);
            }
            
            BundleTable.Bundles.RegisterTemplateBundles();
        }

        private static void AddContent(string path, char contentType, char locationType, IEnumerable<Bundle> bundles)
        {
            foreach (var bundle in bundles) {
                switch (locationType) {
                    case 'r':
                        bundle.AddDirectory(path, (contentType == 'j') ? "*.js" : "*.css", true);
                        break;
                    case 'd':
                        bundle.AddDirectory(path, (contentType == 'j') ? "*.js" : "*.css", false);
                        break;
                    case 'f':
                        bundle.AddFile(path);
                        break;
                }
            }
        }
    }
}