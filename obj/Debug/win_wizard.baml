   M S B A M L   `   `   `  ����D  @TOEC Dist, Version=2.7.0.5, Culture=neutral, PublicKeyToken=null%clr-namespace:TOEC_Dist	TOEC_Dist  5   	       TOEC_Dist.win_wizard   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:TOEC_Dist     5   	   $	��450��5   	   $	��800��6     � WindowStyle$
  None=�5	   	    � AllowsTransparency . $��0q�5   	   
������  � WindowStartupLocation$ CenterScreen=�5   	   $	��450��5   	   $	��800��6    � 
ResizeMode$ 	CanResize=�5   	    � 	Resources 5      $   TOEC_Dist.StringConverterColor   StringConverterColor��%   &	          5   
   6%   5      .��5      ��   panel-default#��[ 6   .��5   
   �� .+�5      �� -   5          panel-heading-default#��[ 5      .��5      � $��25��6   .+�5      �� -   5         ��Name$ tbk_ver��5      $��Install Shield V ��6-   $
��Left=�6I     h5#���  6d   $��5q�6�   6�   �� 5      -   5         ��Name$ 	btn_Close��5   W   $��	550,2,0,2q�65   $��10(�6I     
btn-danger#���  6j   .��6�   X6�   5      5      �� 5        
panel-body#��[ 6   $��5q�6A   
�������.��5      �� .+�5      �� -   5         ��Name$ tc_main��5   !   $	��370��62   $��	0,-20,0,0q�6?   
������  5       �� 
TargetType	 �� Property
 �� Value��%V   '��       �� 5      " ��6$   .�5    "   �� 	 � y�$
 Hidden=�6?   5!      5"      .@�5#      �� $��信息输入��6"   .��5$      � 
������� � ColumnDefinitions 5%   "   �� 5&   &   $��* �67   6C   �� 5'   &   $��60 �67   6D   �� 5(   &   $��* �67   6C   �� 5)   &   $��* �67   6C    � RowDefinitions 5+   "   �� 5,   &   $��* �64   6A   �� 5-   &   $
��50px �64   6D   �� 5.   &   $
��50px �64   6D   �� 5/   &   $
��50px �64   6D   �� 50   &   $��* �64   6A   .+�52   "   �� $��站名��6(   $��1��65   $��1��6E   $��Center=�6R     h4#���  6m   6�   �� 53   "   -   5         ��Name$ txt_stnm��53   *   $��Center=�6=   $	��180��6X   $��0��6e   $��2��6r   $��1��6�   6�   �� 54   "   $��	电报码��6(   $��1��66   $��2��6F   $��Center=�6S   #���  6n   6�   �� 55   "   -   5        $ 	txt_tcode��55   *   $��Center=�6>   $	��180��6Y   $��2��6e   $��2��6u   �� 56   "   $��IP��6(   $��1��65   $��3��6E   $��Center=�6R   #���  6m   6�   �� 57   "   -   5        $ txt_ip��57   *   $��Center=�6;   $	��180��6W   $��1��6c   $��2��6p   $��3��6�   58      59      �� 5:      $��模板编辑��6"   .��5;      &   System.Windows.Controls.DataGrid  -   5          Name$ dg_template��5;   '   $��Stretch=�6<   $��Stretch=�5<         CanUserAddRows .  ��5;   a   ��  �� Path$ src_template�   Columns 5=   "   0  *System.Windows.Controls.DataGridTextColumn  5>   &   ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6g      Header$ 部署名称��6q      CanUserSort .    DataGridTextCenter1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [ 6�      Binding 69   �� *	comment+6�     5?   &    . $ * 6d   $ 部署软件名称��6n    .  # [ 6�    69   �� *name+6�   4  .System.Windows.Controls.DataGridCheckBoxColumn  5@   &   $
 0.4* 6Z    .    DataGridCheckBoxCenter# [ 6{    .  6=   �� *
IsEnable+   HeaderTemplate 5A   *   �� 5B   .   .R�5C   2   ��  ��Name$ ckbSelectedAll��6;   -	   5        $��是否部署��5C   S    d� 	IsChecked$
 TrueY�6�   5D   /   5F   '   5H      5I      �� 5J      $��一键部署��6"   .��5K           
IsReadOnly . ��6'   �� $ 
src_report� 5L   "     5M   &   # [ 6S   $ 进度信息��6�   $ * 6�    .   6:   �� *info+   	CellStyle 5N   *   �� 5O   .   .�5Q   2   �� 	 �@^�ToolTip
 6[   �� *info+5R   /   5T   '   4  .System.Windows.Controls.DataGridTemplateColumn  5U   &   $ 结果状态��6=    .     CellTemplate 5V   *   �� 5W   .   .R�5X   2   �� $��Center=�6�   ��6<   �� *status+��6T   �� *status+ �� 	Converter# [  6�   5Y   /   5[   '   5]      5^      5_      � 5`      $��40��6    $��Bottom=�6,   $��RightToLeft=�6G   .+�5a      �� -
   5        $ btn_next��5a   J   |�.  $��5q�6\    	 btn-primary#��� 	 6g   .��6�   	下一步6�   �� 5b      -   5        $ btn_pre��5b   I   |�.  $��5q�6Z   #��� 	 6e   .��6�   	上一步6�   �� 5c      -   5        $ 
btn_manual��5c   :   $��5q�6N   #���  6Y   .��6~   	预安装6�   �� 5d      -   5        $ 
btn_deploy��5d   !   $��5q�6N    
 btn-success#��� 
 6Y   $y�Hidden=�6   .��6�   安装6�   5e      5f      5g      5h      5i      5j      