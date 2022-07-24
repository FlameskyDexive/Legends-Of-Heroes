# ET-EUI
基于ET框架的简单UI模块

#### 教程视频
- [01-EUI介绍与UI控件获取](https://www.bilibili.com/video/BV12F411e7bP?share_source=copy_web) 

- [02-EUI的公共UI的创建与使用](https://www.bilibili.com/video/BV1VP4y137Ah?share_source=copy_web)   

- [03-EUI的循环列表的创建与使用](https://www.bilibili.com/video/BV1UF411z7uu?share_source=copy_web)   

- [04-EUI的红点组件的创建与使用](https://www.bilibili.com/video/BV1KL4y1p7eh?share_source=copy_web) 

#### 模块特点
 - 完全符合ET框架编码规范，逻辑层与显示层进行分离，代码简洁可阅读性强
 
 - UI预设物无需任何MonoBehaviour脚本挂载
 
 - 完全自主可控的UI界面生命周期
 
 - 无需手动创建脚本、声明方法、变量、拖拽物体赋值。全部一键自动生成。
 
 - 提供实例异步加载，预加载，释放回收等接口，高性能，不卡顿，无顿帧感，享受丝滑的UI界面游玩体验
 
 - 提供在商业化游戏开发中常见的UI功能解决方案


#### 使用方式

-  拼好UI预设物，选择UI物体右键点击SpawnEUICode选项生成UI绑定代码  

-  无需挂任何脚本，无需拖拽任何组件，无需关心组件类型

-  直接开始编写UI业务逻辑，使用公共UI与Item扩展轻松下沉业务逻辑

-  提供统一的UI窗口显示，隐藏，完全关闭，预加载，卸载等接口

-  需要生成的UI窗口以Dlg开头，UI窗口放入Dlg目录

-  需要绑定的UI组件以E开头  

-  需要生成的公共UI以ES开头，公共UI放入Common目录

-  需要生成的循环列表项以Item_开头，滚动项放入Item目录

-  需要生成的空组件物体的引用以EG开头  

-  红点系统加入

#### TODD
1.  UIPropTips系统
2.  切换控制器系统


