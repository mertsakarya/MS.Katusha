using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Katusha.Web.Models
{
    public class GoogleAnalytics
    {
        private readonly IList<IGoogleAnalyticsContent> _items;

        public GoogleAnalytics() {
            _items = new List<IGoogleAnalyticsContent>();
        }
        public void AddPageLevelVariable(GoogleAnalyticsPageLevelVariableType type, string value) { _items.Add(new GoogleAnalyticsCustomVariable { Slot = (byte)type, Name = Enum.GetName(typeof(GoogleAnalyticsPageLevelVariableType), type), ScopeType = ScopeType.Page, Value = value }); }
        public void AddVisitorLevelVariable(GoogleAnalyticsVisitorLevelVariableType type, string value) { _items.Add(new GoogleAnalyticsCustomVariable { Slot = (byte)type, Name = Enum.GetName(typeof(GoogleAnalyticsVisitorLevelVariableType), type), ScopeType = ScopeType.Visitor, Value = value }); }
        public void AddSessionLevelVariable(GoogleAnalyticsSessionLevelVariableType type, string value) { _items.Add(new GoogleAnalyticsCustomVariable { Slot = (byte)type, Name = Enum.GetName(typeof(GoogleAnalyticsSessionLevelVariableType), type), ScopeType = ScopeType.Session, Value = value }); }

        public string ClickLoginEvent() { return new GoogleAnalyticsEvent {Category = "User", Action = "Login"}.ToString(); }
        public string ClickLogoutEvent(Guid guid) { return new GoogleAnalyticsEvent { Category = "User", Action = "Logout", Label = guid.ToString()}.ToString(); }
        public string ClickRegisterEvent() { return new GoogleAnalyticsEvent { Category = "User", Action = "Register" }.ToString(); }
        public string ClickSendMessageEvent(string fromUser ) { return new GoogleAnalyticsEvent { Category = "Message", Action = "Send", Label = fromUser}.ToString(); }
        public string ClickReadMessageEvent(string toUser) { return new GoogleAnalyticsEvent { Category = "Message", Action = "Read", Label = toUser }.ToString(); }
        public string ClickAddPhotosEvent(Guid guid) { return new GoogleAnalyticsEvent { Category = "Photo", Action = "Add", Label = guid.ToString()}.ToString(); }
        public string ClickPhotoAlbumEvent(Guid guid) { return new GoogleAnalyticsEvent { Category = "Photo", Action = "View", Label = guid.ToString() }.ToString(); }
        public string ClickPhotoDeleteEvent(Guid guid) { return new GoogleAnalyticsEvent { Category = "Photo", Action = "Delete", Label = guid.ToString() }.ToString(); }
        public string ClickPhotoMakeProfilePhotoEvent(Guid guid) { return new GoogleAnalyticsEvent { Category = "Photo", Action = "MakeProfilePhoto", Label = guid.ToString() }.ToString(); }

        public override string ToString()
        {
            if (_items.Count == 0) return "";
            var sb = new StringBuilder();
            foreach (var item in _items)
                sb.AppendLine(item.ToString());
            return sb.ToString();
        }
    }


    public class GoogleAnalyticsEvent : IGoogleAnalyticsContent
    {
        public GoogleAnalyticsEvent() { 
            Value = 0;
            AffectsBounceRate = false;
            Label = "";
        }

        public string Category { get; set; }
        public string Action { get; set; }
        public string Label { get; set; }
        public int Value { get; set; }
        public bool AffectsBounceRate { get; set; }

        public override string ToString() { return String.Format("_trackEvent('{0}', '{1}', '{2}', {3}, {4});", Category, Action, Label, Value, AffectsBounceRate ? "true" : "false"); }
        //public override string ToString() { return String.Format("_gaq.push(['_trackEvent', '{0}', '{1}', '{2}', {3}, {4}]);", Category, Action, Label, Value, AffectsBounceRate ? "true" : "false"); }
    }

    public interface IGoogleAnalyticsContent
    {
        string ToString();
    }

    public class GoogleAnalyticsCustomVariable : IGoogleAnalyticsContent
    {
        public ScopeType ScopeType { get; set; }
        public byte Slot { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return String.Format("_gaq.push(['_setCustomVar',{0},'{1}','{2}',{3}]);", Slot, Name, Value, (byte)ScopeType);
        }
    }
    public enum GoogleAnalyticsVisitorLevelVariableType : byte {Gender = 1, CategoryType=2}
    public enum GoogleAnalyticsPageLevelVariableType : byte {Product=1}
    public enum GoogleAnalyticsSessionLevelVariableType : byte {Login = 1}

    public enum ScopeType : byte {Visitor = 1, Session = 2, Page = 3}
}