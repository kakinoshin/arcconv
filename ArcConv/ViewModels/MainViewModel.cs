using ArcConv.Common;
using GongSolutions.Wpf.DragDrop;
using Modules.Views.Common;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
#if IMAGE_RESIZER
using ImageResizer;
#endif
#if IMAGE_SHARP
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;
using Size = SixLabors.Primitives.Size;
using System.Threading.Tasks;
#endif

namespace ArcConv.ViewModels
{
    public class MainViewModel : ViewModelBase, IDropTarget
    {
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

        #region implementation of OutFileName property with Notification
        private string _OutFileName = null;
        public string OutFileName
        {
            set
            {
                if (_OutFileName != value)
                {
                    _OutFileName = value;
                    NotifyPropertyChanged("OutFileName");
                }
            }
            get
            {
                return _OutFileName;
            }
        }
        #endregion

        // Resize

        #region implementation of IsResize property with Notification
        private bool _IsResize = false;
        public bool IsResize
        {
            set
            {
                if (_IsResize != value)
                {
                    _IsResize = value;
                    NotifyPropertyChanged("IsResize");
                }
            }
            get
            {
                return _IsResize;
            }
        }
        #endregion

        #region implementation of ImageWidth property with Notification
        private int _ImageWidth = 800;
        public int ImageWidth
        {
            set
            {
                if (_ImageWidth != value)
                {
                    _ImageWidth = value;
                    NotifyPropertyChanged("ImageWidth");
                }
            }
            get
            {
                return _ImageWidth;
            }
        }
        #endregion

        #region implementation of ImageHeight property with Notification
        private int _ImageHeight = 1280;
        public int ImageHeight
        {
            set
            {
                if (_ImageHeight != value)
                {
                    _ImageHeight = value;
                    NotifyPropertyChanged("ImageHeight");
                }
            }
            get
            {
                return _ImageHeight;
            }
        }
        #endregion

        // Jpeg Compress

        #region implementation of IsCompress property with Notification
        private bool _IsCompress = false;
        public bool IsCompress
        {
            set
            {
                if (_IsCompress != value)
                {
                    _IsCompress = value;
                    NotifyPropertyChanged("IsCompress");
                }
            }
            get
            {
                return _IsCompress;
            }
        }
        #endregion

        #region implementation of JpegQuality property with Notification
        private int _JpegQuality = 30;
        public int JpegQuality
        {
            set
            {
                if (_JpegQuality != value)
                {
                    _JpegQuality = value;
                    NotifyPropertyChanged("JpegQuality");
                }
            }
            get
            {
                return _JpegQuality;
            }
        }
        #endregion

        #region implementation of IsGrayscale property with Notification
        private bool _IsGrayscale = false;
        public bool IsGrayscale
        {
            set
            {
                if (_IsGrayscale != value)
                {
                    _IsGrayscale = value;
                    NotifyPropertyChanged("IsGrayscale");
                }
            }
            get
            {
                return _IsGrayscale;
            }
        }
        #endregion

        // Preview

        #region implementation of SelectedImageWidth property with Notification
        private int _SelectedImageWidth = 0;
        public int SelectedImageWidth
        {
            set
            {
                if (_SelectedImageWidth != value)
                {
                    _SelectedImageWidth = value;
                    NotifyPropertyChanged("SelectedImageWidth");
                }
            }
            get
            {
                return _SelectedImageWidth;
            }
        }
        #endregion

        #region implementation of SelectedImageHeight property with Notification
        private int _SelectedImageHeight = 0;
        public int SelectedImageHeight
        {
            set
            {
                if (_SelectedImageHeight != value)
                {
                    _SelectedImageHeight = value;
                    NotifyPropertyChanged("SelectedImageHeight");
                }
            }
            get
            {
                return _SelectedImageHeight;
            }
        }
        #endregion

        #region [Command: ImageCloseCommand]

        private ICommand _ImageCloseCommand = null;
        public ICommand ImageCloseCommand
        {
            get
            {
                if (_ImageCloseCommand == null)
                {
                    _ImageCloseCommand = new DelegateCommand
                    {
                        ExecuteHandler = ImageCloseCommandExecute,
                        CanExecuteHandler = ImageCloseCommandCanExecute,
                    };
                }

                return _ImageCloseCommand;
            }
        }

        private void ImageCloseCommandExecute(object parameter)
        {
            SelectedImgFilePath = null;
        }

        private bool ImageCloseCommandCanExecute(object parameter)
        {
            return true;
        }

        #endregion

        //

        #region [Command: FileSelectCommand]

        private ICommand _FileSelectCommand = null;
        public ICommand FileSelectCommand
        {
            get
            {
                if (_FileSelectCommand == null)
                {
                    _FileSelectCommand = new DelegateCommand
                    {
                        ExecuteHandler = FileSelectCommandExecute,
                        CanExecuteHandler = FileSelectCommandCanExecute,
                    };
                }

                return _FileSelectCommand;
            }
        }

        private void FileSelectCommandExecute(object parameter)
        {
            System.Console.WriteLine("Push: ...");
        }

        private bool FileSelectCommandCanExecute(object parameter)
        {
            return true;
        }

        #endregion

        #region [Command: ZipOutCommand]

        private ICommand _ZipOutCommand = null;
        public ICommand ZipOutCommand
        {
            get
            {
                if (_ZipOutCommand == null)
                {
                    _ZipOutCommand = new DelegateCommand
                    {
                        ExecuteHandler = ZipOutCommandExecute,
                        CanExecuteHandler = ZipOutCommandCanExecute,
                    };
                }

                return _ZipOutCommand;
            }
        }

        private void ZipOutCommandExecute(object parameter)
        {
            if(!string.IsNullOrEmpty(OutFileName))
            {
                convertToZip(OutFileName);
            }
            else
            {
                MessageBox.Show("Set output file name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ZipOutCommandCanExecute(object parameter)
        {
            return true;
        }

        #endregion

        // Progress
        #region implementation of IsVisibleProgress property with Notification
        private Visibility _IsVisibleProgress = Visibility.Hidden;
        public Visibility IsVisibleProgress
        {
            set
            {
                if (_IsVisibleProgress != value)
                {
                    _IsVisibleProgress = value;
                    NotifyPropertyChanged("IsVisibleProgress");
                }
            }
            get
            {
                return _IsVisibleProgress;
            }
        }
        #endregion

        #region implementation of ProgressOutFileName property with Notification
        private string _ProgressOutFileName = null;
        public string ProgressOutFileName
        {
            set
            {
                if (_ProgressOutFileName != value)
                {
                    _ProgressOutFileName = value;
                    NotifyPropertyChanged("ProgressOutFileName");
                }
            }
            get
            {
                return _ProgressOutFileName;
            }
        }
        #endregion

        #region implementation of ProgressFileName property with Notification
        private string _ProgressFileName = null;
        public string ProgressFileName
        {
            set
            {
                if (_ProgressFileName != value)
                {
                    _ProgressFileName = value;
                    NotifyPropertyChanged("ProgressFileName");
                }
            }
            get
            {
                return _ProgressFileName;
            }
        }
        #endregion

        #region implementation of ProgressValue property with Notification
        private int _ProgressValue = 0;
        public int ProgressValue
        {
            set
            {
                if (_ProgressValue != value)
                {
                    _ProgressValue = value;
                    NotifyPropertyChanged("ProgressValue");
                }
            }
            get
            {
                return _ProgressValue;
            }
        }
        #endregion

        #region implementation of ProgressMax property with Notification
        private int _ProgressMax = 0;
        public int ProgressMax
        {
            set
            {
                if (_ProgressMax != value)
                {
                    _ProgressMax = value;
                    NotifyPropertyChanged("ProgressMax");
                }
            }
            get
            {
                return _ProgressMax;
            }
        }
        #endregion

        #region implementation of IsEnableProgressClose property with Notification
        private bool _IsEnableProgressClose = false;
        public bool IsEnableProgressClose
        {
            set
            {
                if (_IsEnableProgressClose != value)
                {
                    _IsEnableProgressClose = value;
                    NotifyPropertyChanged("IsEnableProgressClose");
                }
            }
            get
            {
                return _IsEnableProgressClose;
            }
        }
        #endregion

        #region [Command: ProgressCloseCommand]

        private ICommand _ProgressCloseCommand = null;
        public ICommand ProgressCloseCommand
        {
            get
            {
                if (_ProgressCloseCommand == null)
                {
                    _ProgressCloseCommand = new DelegateCommand
                    {
                        ExecuteHandler = ProgressCloseCommandExecute,
                        CanExecuteHandler = ProgressCloseCommandCanExecute,
                    };
                }

                return _ProgressCloseCommand;
            }
        }

        private void ProgressCloseCommandExecute(object parameter)
        {
            IsVisibleProgress = Visibility.Hidden;
        }

        private bool ProgressCloseCommandCanExecute(object parameter)
        {
            return true;
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

            ImageData = null;
            ImgFileList.Clear();
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

        private void convertToZip(string filename)
        {
            if (string.IsNullOrEmpty(SelectedFilePath))
            {
                return;
            }

            if (File.Exists(filename))
            {
                if(MessageBox.Show("Overwrite?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(filename);
                }
                else
                {
                    return;
                }
            }

            Task<int> task1 = Task.Run(() => {
                IsVisibleProgress = Visibility.Visible;
                IsEnableProgressClose = false;

                using (var zip = File.OpenWrite(filename))
                using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, CompressionType.Deflate))
                {
                    ProgressOutFileName = SelectedFilePath;

                    IArchive archive = ArchiveFactory.Open(SelectedFilePath);
                    var entries = archive.Entries.Where(e =>
                        e.IsDirectory == false && (
                        Path.GetExtension(e.Key).Equals(".jpg") ||
                        Path.GetExtension(e.Key).Equals(".jpeg") ||
                        Path.GetExtension(e.Key).Equals(".png") ||
                        Path.GetExtension(e.Key).Equals(".bmp")));

                    var list = entries.ToList();
                    ProgressMax = list.Count;
                    ProgressValue = 0;
                    foreach (var item in entries.ToList())
                    {
                        ProgressFileName = item.Key;
                        var outStream = new MemoryStream();
#if IMAGE_RESIZER
                    var instructions = new Instructions()
                    {
                        Width = 300,
                        Mode = FitMode.Max,
                        Format = "jpg"
                    };
                    ImageBuilder.Current.Build(new ImageJob(item.OpenEntryStream(), outStream, instructions, false, true));
                    //item.OpenEntryStream().CopyTo(inStream);
                    //ImageJob imageJob = new ImageJob(inStream, outStream, instructions);
                    //inStream.Seek(0, SeekOrigin.Begin);
                    //ImageBuilder.Current.Build(new ImageJob(item.OpenEntryStream(), outStream, new Instructions("height=300"), false, true));
#endif
#if IMAGE_SHARP
                        using (Image image = Image.Load(item.OpenEntryStream()))
                        {
                            if (IsResize)
                            {
                                image.Mutate(x => x
                                     .Resize(new ResizeOptions
                                     {
                                         Mode = ResizeMode.Min,
                                         Size = new Size(ImageWidth, ImageHeight),
                                     })
                                     );
                            }

                            if (IsCompress)
                            {
                                if (IsGrayscale)
                                {
                                    image.Mutate(x => x
                                         .Grayscale()
                                         );
                                }

                                image.SaveAsJpeg(outStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = JpegQuality, Subsample = SixLabors.ImageSharp.Formats.Jpeg.JpegSubsample.Ratio420 });
                            }
                            else
                            {
                                image.SaveAsJpeg(outStream);
                            }
                        }
#endif

                        outStream.Seek(0, SeekOrigin.Begin);
                        zipWriter.Write(item.Key, outStream);

                        ProgressValue++;
                    }
                }

                IsEnableProgressClose = true;
                return 1;
            });
        }

        private void updateImage()
        {
            if (SelectedImgFilePath == null)
            {
                ImageData = null;
                SelectedImageWidth = 0;
                SelectedImageHeight = 0;
            }
            else
            {
                try
                {
                    using (Image i = Image.Load(SelectedImgFilePath.OpenEntryStream()))
                    {
                        SelectedImageWidth = i.Width;
                        SelectedImageHeight = i.Height;
                    }
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = SelectedImgFilePath.OpenEntryStream();
                    img.EndInit();
                    ImageData = img;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());

                    ImageData = null;
                    SelectedImageWidth = 0;
                    SelectedImageHeight = 0;
                }
            }
        }
    }
}
