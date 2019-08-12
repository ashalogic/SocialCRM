using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SocialCRM_UWP.Utility
{
    public static class Helper
    {
        public static Visibility VisibiltyConverter(bool _input)
        {
            switch (_input)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public static async Task DeleteTempFiles(StorageFolder folder, string fileNameStartsWith)
        {
            var files = (await folder.GetFilesAsync()).Where(p => p.DisplayName.StartsWith(fileNameStartsWith));

            foreach (var file in files)
            {
                await file.DeleteAsync(StorageDeleteOption.Default);
            }
        }
        public static async Task ShowMessage(string title, string content)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "بستن",
            };

            ContentDialogResult result = await contentDialog.ShowAsync();
        }
    }
}
