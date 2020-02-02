using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Windows;
using ArcConv.Common;
using Modules.Views.Common;
using SharpCompress.Archives;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ArcConv.ViewModels
{
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        public String TestStr { get; set; } = "てきとうな初期値";
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();
        public ObservableCollection<IArchiveEntry> ImgFileList { get; } = new ObservableCollection<IArchiveEntry>();
        public string FilePath { set; get; }

        #region implementation of SelectedFilePath property with Notification
        private string _SelectedFilePath = null;
        public string SelectedFilePath
        {
            set
            {
                if (_SelectedFilePath != value)
                {
                    _SelectedFilePath = value;
                    NotifyPropertyChanged("SelectedFilePath");

                    updateImgFileList();
                }
            }
            get
            {
                return _SelectedFilePath;
            }
        }
        #endregion

        #region implementation of SelectedImgFilePath property with Notification
        private IArchiveEntry _SelectedImgFilePath = null;
        public IArchiveEntry SelectedImgFilePath
        {
            set
            {
                if (_SelectedImgFilePath != value)
                {
                    _SelectedImgFilePath = value;
                    NotifyPropertyChanged("SelectedImgFilePath");

                    updateImage();
                }
            }
            get
            {
                return _SelectedImgFilePath;
            }
        }
        #endregion

        #region implementation of ImageData property with Notification
        private BitmapImage _ImageData = new BitmapImage();
        public BitmapImage ImageData
        {
            set
            {
                if (_ImageData != value)
                {
                    _ImageData = value;
                    NotifyPropertyChanged("ImageData");
                }
            }
            get
            {
                return _ImageData;
            }
        }
        #endregion

        public void DragOver(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = files.Any(fname => fname.EndsWith(".zip", ".rar"))
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            FileList.Clear();

            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>()
                .Where(fname => fname.EndsWith(".zip", ".rar")).ToList();

            if (files.Count == 0) return;

            foreach (var file in files)
                FileList.Add(file);
        }

        private void updateImgFileList()
        {
            if(!string.IsNullOrEmpty(SelectedFilePath))
            {
                ImgFileList.Clear();

                IArchive archive = ArchiveFactory.Open(SelectedFilePath);
                var entries = archive.Entries.Where(e =>
                    e.IsDirectory == false && (
                    Path.GetExtension(e.Key).Equals(".jpg") ||
                    Path.GetExtension(e.Key).Equals(".jpeg") ||
                    Path.GetExtension(e.Key).Equals(".png") ||
                    Path.GetExtension(e.Key).Equals(".bmp")));

                foreach(var item in entries.ToList())
                {
                    ImgFileList.Add(item);
                }
            }
        }

        private void updateImage()
        {
            try
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = SelectedImgFilePath.OpenEntryStream();
                img.EndInit();
                ImageData = img;
           }
            catch(Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
