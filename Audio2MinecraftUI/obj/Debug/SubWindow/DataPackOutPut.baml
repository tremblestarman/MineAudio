   M S B A M L   `   `   `  ����L  HAudio2MinecraftUI, Version=1.1.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:Audio2MinecraftUI.SubWindowAudio2MinecraftUI.SubWindow  5   	   H DMahApps.Metro, Version=1.6.5.1, Culture=neutral, PublicKeyToken=nullV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls 5   	   0    *Audio2MinecraftUI.SubWindow.DataPackOutPut   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ;local)clr-namespace:Audio2MinecraftUI.SubWindow     5   	   PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5   	   -   5          � Title$  导出为数据包��5
   	    � Icon$ /img/audio2minecraft.ico��6   $��143.685��68   $	��300��6I   $	��112��5   	   $	��200��6   $	��112��6)   $	��300��68   $��2q�5   	    � 
ResizeMode$ CanMinimize=�5   	    � WindowStartupLocation$ CenterOwner=�5   	   .��5      � .+�5   
   �� -   5         ��Name$
 Path��5      *  $MahApps.Metro.Controls.TextBoxHelper   	Watermark$0 *请选择导出位置 (datapacks文件夹)��6    $
��Left=�6Z   $��24��6u   $��	13,10,0,0q�6�   $	��Top=�6�   $	��272��6�   $�� ��6�   6�   �� 5   
   -   5        $ Name1��5      $ 请输入数据包名称��6!   $
��Left=�6M   $��24��6h   $��	63,44,0,0q�6t   $	��Top=�6�   $	��172��6�   $�� ��6�   6�   �� 5   
   $��命令长度限制��6   $��14,82,190,11q�6"   6:   �� 5   
   -   5        $	 Max��5      $  ��6   $
��Left=�6C   $��24��6^   $��	94,77,0,0q�6j   $	��Top=�6}   $��73��6�   $��65536��6�   6�   �� 5   
   -   5         ��Name$ 禁用全局变量��5      
 J� Uid$ 
EnableLeft��6#   $��Center=�64   $��Center=�6Q   $��186,80,-21,10q�6l    d� 	IsChecked$
 TrueY�6�   $	��125��6�   $��21��6�   .��5      �� $��禁用全局变量��6   	 � ToolTip	 5      _� 5      .��5      �� QO禁用所有全局变量，包括CurrentTick(在命令流中会被强制开启)5      5      5      5      �� 5   
   -   5        $
��Left=�5      $��	13,41,0,0q�6,   $	��Top=�6?   $��32��6W   $��32��6c   .��5       �� -   5        
 ��Name$
 Switcher��5       $��/img/track_view.png��6&   $
��Fill=�6D   $��32��6S   $��32��6_    J� RenderTransformOrigin$ 0.5,0.5&�6j   $��-1,0,-11,-10q�6�   z�5!      V� 5"      .~�5#      �� �� 5$      �� 5%      T� 5&       T� X$ -5Q�6-    T� Y$ -7Q�64   5'      	 5)      _� 5*      .��5+      �� &$切换为音轨导出 / 乐器导出5-      5.      50      51      �� 52   
   -   5         ��Name$ OK��52      $
��Left=�6   $��
253,41,0,0q�68   $	��Top=�6L   $��32��6d   $��32��6p   .��53      �� $��/img/done.png��6   $
��Fill=�6,   $��32��6;   $��32��6G   $ 0.5,0.5&�6R   $��-1,0,-10,-14q�6r   z�54      V� 55      .~�56      �� �� 57      �� 58      T� 59      $ -5Q�6-   $ -7Q�64   5:      	 5<      _� 5=      .��5>      �� 确认修改5@      5A      5C      5D      5E      5F      