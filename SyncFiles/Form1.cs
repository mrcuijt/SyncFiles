﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
                {
                    ProjectPath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                projectPaths.Text = openFileDialog1.FileName;
            }
        }

        private void MoveFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProjectPath.Text))
            {
                MessageBox.Show("请选择同步项目");
                return;
            }
            if (string.IsNullOrEmpty(projectPaths.Text))
            {
                MessageBox.Show("请选择同步位置");
                return;
            }

            Task.Run(() =>
               {
                   try
                   {
                       using (StreamReader sr = new StreamReader(projectPaths.Text))
                       {
                           while (!sr.EndOfStream)
                           {
                               string dirName = sr.ReadLine();
                               toolStripStatusLabel1.Text = "正在同步：" + dirName.Substring(dirName.LastIndexOf("\\")+1);
                               copyDirectory(ProjectPath.Text, dirName);
                           }
                       }
                       toolStripStatusLabel1.Text = "同步完成";
                   }
                   catch(Exception ex)
                   {
                       toolStripStatusLabel1.Text = ex.Message;
                   }
               });
        }

         public static void copyDirectory(string sourceDirectory, string destDirectory)
        {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            copyFile(sourceDirectory, destDirectory);
           
            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);
           
            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);
               
                //递归下去
                copyDirectory(directionPath, directionPathTemp);
            }                     
        }
        public static void copyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);
           
            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
               
                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }    
    }
}