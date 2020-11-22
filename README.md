# CopyDep
Копирование файлов в "красивой" обертке.

### Задача (пример ситуации)
Копирование проекта (сайта) в рабочую директорию. Копирование всех файлов проекта (всего у сайта более 2000 файлов) занимает более 2 минут. Проблема: после замены первого файла (например, cshtml) и до замены последнего файла (например, bin) проходит  долгое время, за которое происходит сверка файлов. По факту, ничего полезного не происходит в этот промежуток времени, но на сайте происходят ошибки (т.к. новое представление уже нуждается в новом коде, например). Задача: составить карту файлов необходимых для замены, и быстрая замена только этих файлов.

### Назначение
Составление карты файлов на замену из директории источника в директорию назначения, для дальнейшего "мгновенного" копирования этих файлов.

![CopyDep Img1](https://github.com/zelderus/CopyDep/blob/master/Docs/copydep_pic_1.png?raw=true)

### Применение
- Указать источник
- Указать назначение
- Подготовка (составление карты файлов)
- Заменить (копирование новых файлов и более новых с заменой)

### Аналоги
Множество, в том числе и аналогичных этому велосипеду.
Есть плагин для Far, есть синхронизация в TotalCommander и другие.

### Todo
- тексты в файлы ресурсов и переводы
- подтверждение на удаление/создание проектов
- информирование, когда файлы в Назначении новее

### Права и контент
icon made by freepic from [flaticon][link_flaticon]

---
**MIT, Free Software by Kirill Kotenko**

[//]: # (Yep)
 [link_web_zedk]: <http://zedk.ru/shcoder>
 [link_flaticon]: <https://www.flaticon.com>
 