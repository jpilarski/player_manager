using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pilarski.PlayerManager.UI.Converters
{
    public class ImagePathToBitmapConverter : IValueConverter
    {
        // Finds the "PlayerManagerData" folder similarly to how BL/DAO does it
        private string GetImagesFolder()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? dir = new DirectoryInfo(currentPath);

            // Go up until we find the solution file (.sln)
            while (dir != null && dir.Parent != null)
            {
                if (dir.GetFiles("*.sln").Length > 0)
                {
                    // Found solution root, look for PlayerManagerData
                    return Path.Combine(dir.FullName, "PlayerManagerData");
                }
                dir = dir.Parent;
            }

            // Fallback (should not happen if structure is correct)
            return currentPath;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? fileName = value as string;

            // If filename is missing, return null (UI will show nothing or broken image)
            if (string.IsNullOrEmpty(fileName))
                return null;

            string folderPath = GetImagesFolder();
            string fullPath = Path.Combine(folderPath, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    // Load image into memory so the file is not locked on disk
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(fullPath);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }

            // Return null if file doesn't exist (UI can handle fallback)
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}