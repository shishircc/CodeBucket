using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Groups
{
    public class GroupInfoController : Controller
    {
        public string User { get; private set; }
        public string GroupName { get; private set; }
        
        public GroupInfoController(string user, string groupName)
            : base(true, true)
        {
            Style = UITableViewStyle.Plain;
            User = user;
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Memebers";
            Title = groupName;
            GroupName = groupName;
        }
        
        protected override void OnRefresh()
        {
            var model = Model as GroupModel;
            var sec = new Section();
            if (model.Members.Count == 0)
            {
                sec.Add(new NoItemsElement("No Members"));
            }
            else
            {
                model.Members.OrderBy(x => x.Username).ToList().ForEach(s => {
                    StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                    sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                    sec.Add(sse);
                });
            }
            
            InvokeOnMainThread(delegate {
                Title = model.Name;
                Root = new RootElement(Title) { sec };
            });
        }
        
        protected override object OnUpdate(bool forced)
        {
            return Application.Client.Users[User].Groups[GroupName].GetInfo(forced);
        }
    }
}

