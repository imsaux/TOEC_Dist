   M S B A M L   `   `   `  ���� D  @TOEC Dist, Version=2.7.0.5, Culture=neutral, PublicKeyToken=null%clr-namespace:TOEC_Dist	TOEC_Dist      TOEC_Dist.win_wizard   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:TOEC_Dist     $	��450��$	��800��  � WindowStyle$
  None=� � AllowsTransparency . $��0q�
������  � WindowStartupLocation$ CenterScreen=�$	��450��$	��800�� � 
ResizeMode$ 	CanResize=� � 	Resources $   TOEC_Dist.StringConverterColor   StringConverterColor��%   &	          .����   panel-default#��[ .���� .+��� -     panel-heading-default#��[ .��� $��25��.+��� -    ��Name$ tbk_ver��$��Install Shield V ��$
��Left=�  h5#���  $��5q��� -    ��Name$ 	btn_Close��$��	550,2,0,2q�$��10(�  
btn-danger#���  .��X��   
panel-body#��[ $��5q�
�������.���� .+��� -    ��Name$ tc_main��$	��370��$��	0,-20,0,0q�
������   �� 
TargetType	 �� Property
 �� Value��%1   '��       �� " ��.��� 	 � y�$
 Hidden=�.@��� $��信息输入��.��� 
������� � ColumnDefinitions �� $��* ��� $��60 ��� $��* ��� $��* � � RowDefinitions �� $��* ��� $
��50px ��� $
��50px ��� $
��50px ��� $��* �.+��� $��站名��$��1��$��1��$��Center=�  h4#���  �� -    ��Name$ txt_stnm��$��Center=�$	��180��$��0��$��2��$��1���� $��	电报码��$��1��$��2��$��Center=�#���  �� -   $ 	txt_tcode��$��Center=�$	��180��$��2��$��2���� $��IP��$��1��$��3��$��Center=�#���  �� -   $ txt_ip��$��Center=�$	��180��$��1��$��2��$��3���� $��模板编辑��.��&   System.Windows.Controls.DataGrid  -     Name$ dg_template��$��Stretch=�$��Stretch=�   CanUserAddRows .  ����  �� Path$ src_template�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ *    Header$ 部署名称��   CanUserSort .    DataGridTextCenter1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [    Binding �� *	comment+   . $ * $ 部署软件名称�� .  # [  �� *name+4  .System.Windows.Controls.DataGridCheckBoxColumn  $
 0.4*  .    DataGridCheckBoxCenter# [  .  �� *
IsEnable+   HeaderTemplate �� .R���  ��Name$ ckbSelectedAll��-	   $��是否部署�� d� 	IsChecked$
 TrueY��� $��一键部署��.��     
IsReadOnly . ���� $ 
src_report�   # [ $ 进度信息��$ *  .   �� *info+   	CellStyle �� .��� 	 �@^�ToolTip
 �� *info+4  .System.Windows.Controls.DataGridTemplateColumn  $ 结果状态�� .     CellTemplate �� .R��� $��Center=����� *status+���� *status+ �� 	Converter# [  � $��40��$��Bottom=�$��RightToLeft=�.+��� -
   $ btn_next��|�.  $��5q� 	 btn-primary#��� 	 .��	下一步�� -   $ btn_pre��|�.  $��5q�#��� 	 .��	上一步�� -   $ 
btn_manual��$��5q�#���  .��	预安装�� -   $ 
btn_deploy��$��5q� 
 btn-success#��� 
 $y�Hidden=�.��安装