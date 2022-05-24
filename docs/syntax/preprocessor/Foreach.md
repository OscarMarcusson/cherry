# Foreach
A foreach statement is used to automate repeated input for a given sequence, like a range of numbers or any files that match a given filter at a given directory. This is useful for things like store pages or other similar situations with a limited set of fixed objects that should all look the same.

## Syntax
There are two sections to a foreach statement; the declaration line, and the body to repeat.

The declaration is written with the following syntax:
```ini
foreach [variable-name] in [type]:[value]
```