# Foreach
A foreach statement is used to automate repeated input for a given sequence, like a range of numbers or any files that match a given filter at a given directory. This is useful for things like store pages or other similar situations with a limited set of fixed objects that should all look the same.

There are two sections to a foreach statement; the declaration line, and the body to repeat.

## Declaration
The declaration is written with the following syntax:
```ruby
foreach [variable-name] in [type]:[value]
```

### Range
To loop through a given number range, in this case 1 - 10 (both inclusive), you would do the following:
```ruby
foreach i in range:1-10
```
The variable i can then be used by the child elements, either directly or as part of some calculation or string creation.


### File
To loop through any files matching a given filter, like part of a name or everything with a certain extension, you use the **file:** type. In this case we'll load all .ini files:
```ruby
foreach f in file:resources/some-data/*.ini
```
We can now use the variable f to get data from the file, like the path, name, extension or raw content, but we can also use the direct accessor : to try and read directly from it's content. This is currently only supported for .ini files, but plans for xml, json, and csv support exist. Let's assume we have a config file like this:
```ini
# Some global data
meaning-of-life = 42

# Some scoped data
[person]
name = Oscar
```
We could then use the content directly in our elements by for example calling ```f:meaning-of-life ``` or ```f:person.name```.

**NOTE** The direct reading is still under development, included here to make sure I don't forget it myself :)
