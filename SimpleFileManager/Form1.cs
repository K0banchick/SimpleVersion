using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

/*Создать двух оконыый файловый менеджер, добавить в функцию Empty() возможность удалять несколько выделенных вайлов и папок
 Создание фалов и папок, а также их переименование, добавить технологию Dragn drop, исправить ошибку с некорректными путями к дискам, а так же заблокировать недоступные или закрытые администратором диски
 добавить второе окно, функции можно клонировать из 1, но выполняться должны они параллельно, оптимизация кода, где это возможно.*/

namespace SimpleFileManager
{
    public partial class Form1 : Form
    {
        public string filePath = @"D:\";
        private bool isFile = false;
        private string currentlySelectedItemName;
        public string currentlySelect;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filePathTextBox.Text = filePath;
            loadFilesAndDirectories();
        }

        public void loadFilesAndDirectories() //узка файлов и папок
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            DirectoryInfo fileList;
            string tempFilePath = "";
            FileAttributes fileAttr;
            try
            {
                
                if (isFile)
                {
                    tempFilePath = filePath + "/" + currentlySelectedItemName;
                    FileInfo fileDetails = new FileInfo(tempFilePath);
                    fileNameLabel.Text = fileDetails.Name;
                    fileTypeLabel.Text = fileDetails.Extension;
                    fileAttr = File.GetAttributes(tempFilePath);
                    Process.Start(tempFilePath);
                }
                else
                {
                    fileAttr = File.GetAttributes(filePath);
                    
                }

                if((fileAttr & FileAttributes.Directory ) == FileAttributes.Directory)
                {
                    
                    fileList = new DirectoryInfo(filePath);                   
                    FileInfo[] files = fileList.GetFiles(); // получение всех файлов
                    DirectoryInfo[] dirs = fileList.GetDirectories(); // получение всех папок
                    string fileExtension = "";
                    listView1.Items.Clear();
                    foreach (DriveInfo d in drives)
                    {
                        listView1.Items.Add(d.Name, 8);

                        if (listView1.SelectedItems.ToString() == d.Name)
                        {
                            filePath = " ";
                            filePath = d.Name + @"\";
                        }
                    }
                    for (int i = 0; i < files.Length; i++)
                    {
                        fileExtension = files[i].Extension.ToUpper();
                        switch(fileExtension)
                        {
                            case ".MP3":
                            case ".MP2":
                                listView1.Items.Add(files[i].Name, 5);
                                break;
                            case ".EXE":
                            case ".COM":
                                listView1.Items.Add(files[i].Name, 7);
                                break;

                            case ".MP4":
                            case ".AVI":
                            case ".MKV":
                                listView1.Items.Add(files[i].Name, 6);
                                break;
                            case ".PDF":
                                listView1.Items.Add(files[i].Name, 4);
                                break;
                            case ".DOC":
                            case ".DOCX":
                                listView1.Items.Add(files[i].Name, 3);
                                break;
                            case ".PNG":
                            case ".JPG":
                            case ".JPEG":
                                listView1.Items.Add(files[i].Name, 9);
                                break;

                            default:
                                listView1.Items.Add(files[i].Name, 8);
                                break;
                        }
                        
                    }

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        listView1.Items.Add(dirs[i].Name, 10);
                    }
                }
                else
                {
                    fileNameLabel.Text = this.currentlySelectedItemName;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Empty(DirectoryInfo direct) // функция рекурсивного удаления файлов и папок
        {
            
            foreach (FileInfo file in direct.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in direct.GetDirectories()) subDirectory.Delete(true);

            direct.GetDirectories(); direct.Delete(true);
        }

        public void loadButtonAction()//реализация перехода в файл
        {
            removeBackSlash();
            filePath = filePathTextBox.Text;
            loadFilesAndDirectories();
            isFile = false;
        }

        public void removeBackSlash()//Удаление части пути до /
        {
            string path = filePathTextBox.Text;
            if(path.LastIndexOf("/") == path.Length - 1)
            {
                filePathTextBox.Text = path.Substring(0, path.Length - 1);
            }
        }

        public void goBack()//реализация возвращения на директорию назад
        {
            try
            {
                removeBackSlash();
                string path = filePathTextBox.Text;
                path = path.Substring(0, path.LastIndexOf("/"));
                this.isFile = false;
                filePathTextBox.Text = path;
                removeBackSlash();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            loadButtonAction();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {

                currentlySelectedItemName = e.Item.Text;

                FileAttributes fileAttr = File.GetAttributes(filePath + "/" + currentlySelectedItemName);
                if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    isFile = false;
                    filePathTextBox.Text = filePath + "/" + currentlySelectedItemName;
                }
                else
                {
                    isFile = true;
                    filePathTextBox.Text = filePath + "/" + currentlySelectedItemName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            loadButtonAction();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            goBack();
            loadButtonAction();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
           /*try
            {
                if(isFile)
                {
                    File.Move(filePathTextBox.Text, folderBrowserDialog1.SelectedPath);
                }

                Directory.Move(filePathTextBox.Text, folderBrowserDialog1.SelectedPath);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/
           
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RightClckMenu.Show(MousePosition, ToolStripDropDownDirection.Right);
            }
        }

        private void deliteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(filePathTextBox.Text);

                    
                    Empty(directory);
                    string path = filePathTextBox.Text;
                    path = path.Substring(0, path.LastIndexOf("/"));
                    filePathTextBox.Text = path;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileInfo Createfile = new FileInfo(CreateFileTxt.Text);
            Createfile.Create();

        }
    }
}
