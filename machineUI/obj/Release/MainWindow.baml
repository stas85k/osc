   M S B A M L   `   `   `  ���� R  NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0>.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.WpfOxyPlot.Wpf  B >machine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:machineUI	machineUI    machineUI.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     >oxy.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.Wpf     )localclr-namespace:machineUI       � Title$  
MainWindow��$��523.861��$��744.661�� � DataContext    machineUI.linearviewmodel1  .��� $��Stretch=�$
��Auto��$��0,0,0,0q�$��Stretch=�$
��Auto�� � RowDefinitions �� $	��18* ��� $
��233* ��� $	��98* � � ColumnDefinitions �� $
��149* ��� $
��548* ��� $	��40* �.+��� -    ��Name$ 
tabControl��$��1��$��Stretch=�$
��Auto��$��0,0,0,0q�$��1��$��Stretch=�$
��Auto��.@��� $��TabItem��.��� ���	#FFE5E5E5�� $��
TickSlider��.��� ���	#FFE5E5E5.+���  �� Orientation$ Vertical=�$
��Left=�$	��Top=�$
��Auto��$	��400��$��	27,27,0,0q�.+��� $��rng  (sub).��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto���� -   $��mySlider��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��$��2q�
����  � � Minimum$ 0Q� � Maximum$	 100Q� �� IsSelectionRangeEnabled . 	 �� SelectionStart$	 10Q�
 �� SelectionEnd$
 70Q� �� TickPlacement$ BottomRight=� �� TickFrequency$ 10Q��� $ 
Horizontal=�$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��.+��� $��Value:��$��Stretch=�$��Stretch=�$��20��$��50���� $��Stretch=�$��Stretch=�$��20��$��50������ *Value+ �� ElementName$ mySlider���� $��Log-Log��$
��Left=�$��21��$	��Top=�$
��Auto��.��� ���	#FFE5E5E5�� $��Lin-Lin��$
��Left=�$��21��$	��Top=�$
��Auto��.��� ���	#FFE5E5E5.+�   OxyPlot.Wpf.Plot  -     Name$ linearview1��   Title$ linear1 plot��   Series    OxyPlot.Wpf.LineSeries  ���� *p1+�� $��	lin lin 2��$
��Left=�$��21��$	��Top=�$
��Auto��.��� ���	#FFE5E5E5.+�  -   $ linearview2��$ linear1 plot 2��   ���� *p2+�� $��TabItem��$
��Left=�$��21��$	��Top=�$
��Auto��.��� ���	#FFE5E5E5$��0,0,-2,0q�� $
��Left=�$	��234��$��1��$	��Top=�$	��149��$��0,10,0,0q� �� $
��Auto �$��1���� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $	��47* ��� $	��10* �.+��� -    ��Name$ 
buttonOpen��$��Open Device��$
��Left=�$��Bottom=�$	��103��$��7,0,0,0q�$
��Auto���� -    ��Name$ 
canvasOpen��
���   �$��1��$��Stretch=�$
��Auto��$��Stretch=�$
��Auto�� J� RenderTransformOrigin$	 0,0&��� -    ��Name$ comboBoxRangeA��$��Stretch=�$��1��$	��Top=�$
��Auto��$
��Auto��$��1��$��0,0,0,0q��� -   $ buttonA��$��A��$��1��$��Stretch=�$��1��$��1��$��21��$	��Top=��� -	   $ comboBoxRangeB��$��Stretch=�$��2��$	��Top=�$
��Auto��$
��Auto��$��1��$��0,0,0,0q��� -
   $ buttonB��$��B��$��1��$��Stretch=�$��0,0,0,0q�$��2��$��1��$��Stretch=��� -   $ comboBoxRangeC��$��Stretch=�$��3��$	��Top=�$
��Auto��$
��Auto��$��1��$��0,0,0,0q��� -   $ buttonC��$��C��$��1��$��Stretch=�$��0,0,0,0q�$��3��$��1��$��Stretch=��� -   $ comboBoxRangeD��$��Stretch=�$��4��$	��Top=�$
��Auto��$
��Auto��$��1��$��0,0,0,0q��� -   $ buttonD��$��D��$��1��$��Stretch=�$��0,0,0,0q�$��4��$��1��$��Stretch=��� -   $ comboBoxRangeG��$��Stretch=�$��5��$	��Top=�$
��Auto��$
��Auto��$��1��$��0,0,0,0q��� -   $ buttonG��$��G��$��1��$��Stretch=�$��0,0,0,0q�$��5��$��1��$��Stretch=��� -    ��Name$ sliderGAmplitude��$��Stretch=�$��0,0,0,0q�$��8��$��Stretch=��� -   $ canvasGAmplitude��
���   �$��1��$��8��$��Stretch=�$
��Auto��$��Stretch=�$
��Auto��$	 0,0&��� -   $ sliderGDCOffset��$��Stretch=�$��0,0,0,0q�$��9��$��Stretch=��� -   $ canvasGDCOffset��
���   �$��1��$��9��$��Stretch=�$
��Auto��$��Stretch=�$
��Auto��$	 0,0&��� -   $ buttonAcquisitionStart��$��Start Acquisition��$��10��$��Stretch=�$	��Top=�$
��Auto��$��7,1,7,0q�$
��Auto���� -   $ canvasAsquisitionStart��
���   �$��1��$��10��$��Stretch=�$
��Auto��$��Stretch=�$
��Auto��$	 0,0&��� -    ��Name$ textBlockLogScroll��$��Stretch=�$
��Auto��$��2��$��Stretch=�$
��Auto��.���� -    ��Name$ textBlockLog��$��Stretch=�$
��Wrap=�$�� ��$��Stretch=�$
��Auto��$
��Auto��