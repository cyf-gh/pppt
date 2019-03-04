using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pppt.Core {
    public class PPPT {
        static string _Title = Guid.NewGuid().ToString();
        static string _FilePath = File.ReadAllText( Path.Combine( System.Windows.Forms.Application.StartupPath, "target.txt" ) ); // 存放位置
        /// <summary>
        /// 创建目标文件夹
        /// </summary>
        static void CreateFileDirectory() {
            if( !Directory.Exists( _FilePath ) ) {
                Directory.CreateDirectory( _FilePath );
            }
        }
        public static void Start( string[] args, string execFileName ) {
            CreateFileDirectory();
            string execPath = "";

            // 读取打开该文件的程序路径
            try {
                execPath = File.ReadAllText( Path.Combine( System.Windows.Forms.Application.StartupPath, execFileName ) );
            } catch {
                return;
            }

            foreach( var arg in args ) {
                try {
                    if( File.Exists( arg ) ) {
                        string targetFileName = Path.Combine( _FilePath, Path.GetFileName( arg ) );
                        // 如果已经复制完了就直接打开
                        if( !File.Exists( targetFileName ) ) {
                            File.Copy( arg, Path.Combine( _FilePath, targetFileName ) );
                            // 保证文件复制的正确性，通常循环不应该执行
                            int MaxCount = 10;
                            while( GetMD5( arg ) != GetMD5( targetFileName ) ) {
                                File.Delete( targetFileName );
                                File.Copy( arg, Path.Combine( _FilePath, targetFileName ) );
                                System.Threading.Thread.Sleep( 100 );
                                MaxCount--;
                                if( MaxCount == 0 ) {
                                    break; // 不知道是什么鬼错误次数实在太多，放弃。
                                }
                            }
                        }

                        string _arg = "\""+ arg +"\"";

                        // 像往常一样启动应用
                        try {
                            Process.Start( execPath, _arg );
                        } catch {
                            RunCmd2( execPath, _arg ); // 备用方案
                        }
                    }
                } catch {
                    continue;
                }
            }
        }
        static string GetMD5( string filePath ) {
            try {
                FileStream file = new FileStream( filePath, FileMode.Open );
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retval = md5.ComputeHash( file );
                file.Close();

                StringBuilder sc = new StringBuilder();
                for( int i = 0; i < retval.Length; i++ ) {
                    sc.Append( retval[i].ToString( "x2" ) );
                }
                Console.WriteLine( "File MD5：{0}", sc );
                return sc.ToString();
            } catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                return "";
            }
        }
        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        static bool RunCmd2( string cmdExe, string cmdStr ) {
            bool result = false;
            try {
                using( Process myPro = new Process() ) {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format( @"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit" );

                    myPro.StandardInput.WriteLine( str );
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();

                    result = true;
                }
            } catch {

            }
            return result;
        }
    }
}
