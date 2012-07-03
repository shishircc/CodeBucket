using System.Drawing;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using BitbucketBrowser.Utils;

namespace BitbucketBrowser.UI
{
    public class RepositoryInfoController : Controller<RepositoryDetailedModel>
    {

        public RepositoryInfoController(RepositoryDetailedModel model)
            : base(true)
        {
            Title = model.Name;
            Model = model;
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Refresh();
        }

        protected override void OnRefresh()
        {
            var lastUpdated = "Updated " + DateTime.Parse(Model.UtcLastUpdated).ToDaysAgo();

            var header = new HeaderView(View.Bounds.Width) { 
                Title = Model.Name, Subtitle = lastUpdated
            };

            if (!string.IsNullOrEmpty(Model.Logo)) {
                var url = new NSUrl(Model.Logo);
                var data = NSData.FromUrl(url);
                header.Image = new UIImage(data);
            }

            Root.Add(new Section(header));
            var sec1 = new Section();
            
            if (!string.IsNullOrEmpty(Model.Description) && !string.IsNullOrWhiteSpace(Model.Description))
            {
                sec1.Add(new MultilineElement(Model.Description));
            }


            var owner = new StyledElement("Owner", Model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(Model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledElement ("Followers", "" + Model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(Model.Owner, Model.Slug), true);
            sec1.Add(followers);


            var events = new CustomImageStringElement("Events", UIImage.FromBundle("Images/repoevents.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            events.Tapped += () => NavigationController.PushViewController(new RepoEventsController(Model.Owner, Model.Slug), true);

            var sec2 = new Section();
            sec2.Add(events);

            if (Model.HasIssues) 
                sec2.Add(new CustomImageStringElement("Issues", () => NavigationController.PushViewController(new IssuesController(Model.Owner, Model.Slug), true),
                                                UIImage.FromBundle("Images/flag")) { Accessory = UITableViewCellAccessory.DisclosureIndicator });

            if (Model.HasWiki)
                sec2.Add(new CustomImageStringElement("Wiki", () => NavigationController.PushViewController(new WikiInfoController(Model.Owner, Model.Slug), true),
                                                UIImage.FromBundle("Images/pencil.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator });

            var sec3 = new Section() {
                new CustomImageStringElement("Changes", () => NavigationController.PushViewController(new ChangesetController(Model.Owner, Model.Slug), true), 
                                       UIImage.FromBundle("Images/commit.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                new CustomImageStringElement("Branches", () => NavigationController.PushViewController(new BranchController(Model.Owner, Model.Slug), true),
                                       UIImage.FromBundle("Images/branch.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
                new CustomImageStringElement("Tags", () => NavigationController.PushViewController(new TagController(Model.Owner, Model.Slug), true),
                                        UIImage.FromBundle("Images/tag.png")) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
            };
            
            Root.Add(new [] { sec1, sec2, sec3 });
        }

        protected override RepositoryDetailedModel OnUpdate()
        {
            return Model;
        }
    }
}