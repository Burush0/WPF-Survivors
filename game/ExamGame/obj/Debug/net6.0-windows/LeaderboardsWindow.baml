   M S B A M L   `   `   `  ����C  ?ExamGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ExamGameExamGame  5   	   !    ExamGame.LeaderboardsWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:ExamGame     5   	     � Cursor$  assets/cursors/menu.cur��5   	    � WindowState$ 	Maximized=�5	   	    � WindowStyle$
 None=�5
   	    � Title$ LeaderboardsWindow��5   	   $
��1080��6$   $
��1920��62   ��5      �� 5   
    �� ImageSource$ assets/images/menu_bg.jpg��6   .��5      � .+�5   
   (� $	��800��6   .��5      �� 
�������   GoldGradient#��[  6'   $��2q�6S    �� CornerRadius$ 5��6g   ._�5      &   System.Windows.Controls.DataGrid  -   5          Name$ leaderboardDataGrid��5         RowHeaderWidth$ 0Q�5         RowBackground
 �����	   AlternatingRowBackground
	 �����
   AutoGenerateColumns$
 False��5         CanUserAddRows$ False��5         CanUserDeleteRows$ False��5         CanUserReorderColumns$ False��5         CanUserResizeRows$ False��5         CanUserSortColumns$ False��5      $��10q�5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   CanUserResize$ False��6-      CanUserSort$ False��6C      CanUserReorder$ False��6W      Header$ Username��6n   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6�   �� *
Username+  5      $ False��6-   $ False��6C   $ False��6W   $ Score��6n    6~   �� *Score+5!      5"      5#      (� 5$   
   $��Right=�6   $	��Top=�6.   $	��400��6F   $	��200��6R   .��5%      �� -   5          PixelButton#��[ 5%   +   $
��Back��6P   $	��100��6_   $	��Top=�6k   $��
0,20,20, 0q�6�   5&      5'      5(      