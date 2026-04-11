package org.example;

import java.io.*;
import java.util.Random;

//TIP 要<b>运行</b>代码，请按 <shortcut actionId="Run"/> 或
// 点击装订区域中的 <icon src="AllIcons.Actions.Execute"/> 图标。
public class Main {
    public static void main(String[] args) throws IOException {
        //TIP 当文本光标位于高亮显示的文本处时按 <shortcut actionId="ShowIntentionActions"/>
        // 查看 IntelliJ IDEA 建议如何修正。

        File file = new File("C:\\Users\\dong\\Desktop\\aaa.txt");
        FileOutputStream fos = new FileOutputStream( file);
        OutputStreamWriter ow = new OutputStreamWriter(fos);
        Random r = new Random();
        for (int i = 0; i < 100000; i++) {
            int a = r.nextInt();
            ow.write(String.valueOf(a));
        }
        fos.close();





        }
    }
