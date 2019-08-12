using SocialCRM_UWP.Instagram.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialCRM_UWP.Instagram.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentsManagement : Page
    {
        public CommentsManagement()
        {
            this.InitializeComponent();
            LoadCommentManagement();
        }

        async Task LoadCommentManagement()
        {
            CommentsManagementPRing.IsActive = true;
            string[] badwords ={
                "بچه کونی",
                "کونی",
                "کون",
                "کیر کلفت",
                "کیری",
                "کیر",
                "کس کش",
                "کوسکش",
                "کوس کش",
                "کسننه",
                "کس ننه",
                "کُس",
                "کوس",
                "کسکش",
                "جنده",
                "قحبه",
                "گاهید",
                "بگا",
                "بگاه",
                "به گا",
                "به گاه",
                "بفنا",
                "به فنا",
                "فاک",
                "فاکر",
                "خفه شو",
                "مرتیکه",
                "زنیکه",
                "جوجو",
                "پستان",
                "پستون",
                "احمق",
                "خنگ",
                "معتاد",
                "انگل",
                "لجن",
                "بیشعور",
                "بی شعور",
                "شهوت",
                "سکس",
                "سکسی",
                "گی",
                "لز",
                "لزبین",
                "همجنس باز",
                "سگ",
                "سگی",
                "گربه",
                "پیشی",
                "الاغ",
                "خریت",
                "کره خر",
                "خر",
                "گاو",
                "گوساله",
                "روانی",
                "دیوانه",
                "دیوونه",
                "تخم",
                "تخمی",
                "پشم",
                "پشمالو",
                "مادر به خطا",
                "حرامزاده",
                "حرام زاده",
                "حرومزاده",
                "حروم زاده",
                "حرامی",
                "آشغال",
                "پفیوز",
                "دیوس",
                "دئیوس",
                "دزد",
                "کلاهبردار",
                "کلاه بردار",
                "فیلترشکن",
                "فیلتر شکن",
                "چیلترشکن",
                "چیلتر شکن",
                "پیلترشکن",
                "پیلتر شکن",
                "فیلتر",
                "چیلتر",
                "پیلتر",
                "قندشکن",
                "قند شکن",
                "چیزمیز",
                "چیز میز",
                "چیزشکن",
                "چیز شکن",
                "وی پی ان",
                "وی-پی-ان",
                "وی.پی.ان",
                "پروکسی",
                "پراکسی",
                "پورن",
                "پورنوگرافی",
                "داف",
                "دافی",
                "پاف",
                "پافی",
                "پیف",
                "ایش",
                "هیس",
                "مبتذل",
                "گه",
                "گوه",
                "اَن",
                "گلابی",
                "خیار",
                "موز",
                "اُبی",
                "اوبی",
                "عشقبازی",
                "عشق بازی",
                "بوسه",
                "هیز",
                "حیز",
                "زارت",
                "زرت",
                "زورت",
                "گوز",
                "گوزید",
                "چس",
                "چُس",
                "جیش",
                "شاش",
                "شاشید",
                "ریدن",
                "ریدمانی",
                "ریدمونی",
                "ریدم",
                "ریدی",
                "رید",
                "ریدیم",
                "ریدید",
                "ریدند",
                "زر",
                "زرزر",
                "زر زر",
                "ور",
                "ورور",
                "ور ور",
                "علاف",
                "الاف",
                "عیاش",
                "لاشی",
                "شراب",
                "مشروب",
                "ویسکی",
                "ودکا",
                "وودکا",
                "عرق",
                "شامپاین",
                "چامپاین",
                "شمپین",
                "چمپین",
                "پیشته",
                "چخه",
                "هش",
                "هُش",
                "ماهواره",
                "ستلایت",
                "بمب",
                "دولت",
                "جمهوری اسلامی",
                "رئیس جمهور",
                "رهبر",
                "خمینی",
                "حامنه",
                "راهپیمایی",
                "تظاهرات",
                "ترور",
                "قتل",
                "قاتل",
                "لامصب",
                "لا مذهب",
                "کافر",
                "بی دین",
                "بی ایمان",
                "بی ایمون",
                "جق",
                "جلق",
                "جلغ",
                "ارگاسم",
                "اورگاسم",
                "جهنم",
                "لعنت",
                "ویاگرا",
                "تورنت",
                "وارز",
                "کثافت",
                "کثافط",
                "کصافت",
                "کصافط",
                "کسافت",
                "کسافط",
                "شورت",
                "کرست",
                "کرصت",
                "کرثت",
                "سوتین",
                "صوتین",
                "ثوتین",
                "fuck",
                "fcuk",
                "son of a bitch",
                "bitch",
                "blow job",
                "boob",
                "cock",
                "cox",
                "deck",
                "cum",
                "kum",
                "gay",
                "lesbian",
                "homosexual",
                "homo-sexual",
                "homo",
                "sex",
                "hell",
                "orgasim",
                "orgasm",
                "porn",
                "piss",
                "shit",
                "damn",
                "tit",
                "vagina",
                "viagra",
                "xxx",
                "ass",
                "filter",
                "philter",
                "vpn",
                "v-p-n",
                "v.p.n",
                "proxy",
                "warez",
                "torrent",
                "shut up"
            };

            var _UserMedias = await Api.InstaApi.GetUserMediaAsync(Api.Username, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(2));
            foreach (var m in _UserMedias.Value)
            {
                var _MediaComments = await Api.InstaApi.GetMediaCommentsAsync(m.InstaIdentifier, InstaSharper.Classes.PaginationParameters.MaxPagesToLoad(10));
                foreach (var c in _MediaComments.Value.Comments)
                {
                    //var _cRefined = SCICT.NLP.Utility.StringUtil.RefineAndFilterPersianWord(c.Text);
                    //string[] __cRefinedExtracted = SCICT.NLP.Utility.StringUtil.ExtractPersianWordsStandardized(_cRefined);
                    string[] __cRefinedExtracted = c.Text.Split(' ');
                    bool isbad = false;
                    foreach (var w in __cRefinedExtracted)
                    {
                        if (badwords.Contains(w))
                        {
                            isbad = true;
                            break;
                        }
                    }
                    if (isbad)
                    {
                        CommentsManagementBList.Items.Add(new CommentViewModel() { CommentId = c.Pk.ToString(), MediaId = m.InstaIdentifier, UserId = c.UserId.ToString(), Date = c.CreatedAt.ToShortDateString(), LikesCount = c.LikesCount.ToString(), UserName = c.User.UserName, ProfilePic = c.User.ProfilePicture, Text = c.Text });
                    }
                    else
                    {
                        CommentsManagementRList.Items.Add(new CommentViewModel() { CommentId = c.Pk.ToString(), MediaId = m.InstaIdentifier, UserId = c.UserId.ToString(), Date = c.CreatedAt.ToShortDateString(), LikesCount = c.LikesCount.ToString(), UserName = c.User.UserName, ProfilePic = c.User.ProfilePicture, Text = c.Text });
                    }
                }
            }
            CommentsManagementPRing.IsActive = false;
        }
    }
}
