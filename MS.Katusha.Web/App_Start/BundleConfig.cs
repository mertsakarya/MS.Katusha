using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Optimization;

namespace MS.Katusha.Web
{
    public class BundleConfig
    {
        private class AsIsBundleOrderer : IBundleOrderer
        {
            public virtual IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files) { return files; }
        }

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.Bundles.EnableDefaultBundles();
            var staticContentCss = new StyleBundle("~/Static/Content/css") {Orderer = new AsIsBundleOrderer()}.Include(
                "~/Static/Content/Fcbk.css",
                "~/Static/Content/Site.css",
                "~/Static/Content/PagedList.css",
                "~/Static/Content/mosaic.css",
                "~/Static/Content/bootstrap-min.css",
                "~/Static/Content/jquery.fileupload-ui.css",
                "~/Static/Content/bootstrap-image-gallery-min.css",
                "~/Static/Content/bootstrap-responsive-min.css"
                );
            bundles.Add(staticContentCss);

            bundles.Add(new StyleBundle("~/Content/themes/base/css") {Orderer = new AsIsBundleOrderer()}.Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.resizable.css",
                "~/Content/themes/base/jquery.ui.selectable.css",
                "~/Content/themes/base/jquery.ui.accordion.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery.ui.button.css",
                "~/Content/themes/base/jquery.ui.dialog.css",
                "~/Content/themes/base/jquery.ui.slider.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/themes/base/jquery.ui.datepicker.css",
                "~/Content/themes/base/jquery.ui.progressbar.css",
                "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/Static/Scripts/js") {Orderer = new AsIsBundleOrderer()}.Include(
                "~/Static/Scripts/jquery-min-1.7.1.js",
                "~/Static/Scripts/jquery-ui-1.8.19-min.js",
                "~/Static/Scripts/bootstrap-min.js",
                "~/Static/Scripts/modernizr-2.5.3.js",
                "~/Static/Scripts/AjaxLogin.js",
                "~/Static/Scripts/knockout-2.1.0.js",
                "~/Static/Scripts/mosaic.1.0.1.js",
                "~/Static/Scripts/jquery.fcbkcomplete.js",
                "~/Static/Scripts/jquery.validate-min.js",
                "~/Static/Scripts/jquery.unobtrusive-ajax.js",

                "~/Static/Scripts/tmpl-min.js",
                "~/Static/Scripts/canvas-to-blob-min.js",
                "~/Static/Scripts/load-image-min.js",

                "~/Static/Scripts/bootstrap-image-gallery-min.js",

                "~/Static/Scripts/jquery.iframe-transport.js",
                "~/Static/Scripts/jquery.fileupload.js",
                "~/Static/Scripts/jquery.fileupload-ip.js",
                "~/Static/Scripts/jquery.fileupload-ui.js",

                "~/Static/Scripts/locale.js",
                "~/Static/Scripts/main.js"
                            ));
            if (!Configuration.KatushaConfigurationManager.Instance.GetSettings().MinifyContent)
                foreach (var bundle in BundleTable.Bundles) {
                    bundle.Transforms.Clear();
                }
        }
    }
}
