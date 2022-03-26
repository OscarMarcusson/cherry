# Element declaration
In its most simple way, elements are declared as `{element}={value}`, or just `{element}` for empty or hierarchical elements.

**Simplified**
```ini
div
p = Hello World
```
**HTML**
```HTML
<div></div>
<p>Hello World</p>
```



<br>

## Value parsing
In the example above the value is written directly, which means it will be parsed as "everything after =", trimmed. If you wish to include spaces or tabs at the start or end of the value you have to wrap it in quotation marks, like this: 
```ini
p = "  String with spaces  "
```
**HTML**
```HTML
<p>&nbsp;&nbsp;String with spaces&nbsp;&nbsp;</p>
```



<br>

## Extended element parsing
Anything that is not a pure text value is defined in the left section of the element declaration. The default is of course the element name, which in many cases also define some type of behaviour like **p**, **h1**, **table** and so on.
