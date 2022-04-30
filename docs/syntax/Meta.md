# Meta
The root meta tag is used to configure the general information about the page, the most common being the title.

Keyword | Description
---|---
title | Sets the page title, in the browser this tends to appear as the tab name. It's recommended to always set this value.
description | Sets the description which is displayed by the search engines results.
author | Sets the author of this page.
search-engine-keywords | A comma separated list of keywords to be used by the search engines.
view-port-scale | Sets the initial page zoom. The default value is 1.

## Example
The following is an example of how a simple "Hello World" page with meta information could be written:

```ini
meta
	title = "Hello World"
	description = "A simple hello world page example."
	author = "Oscar Marcusson"
	search-engine-keywords = "Hello World, Example"
	view-port-scale = 1
body
	h1 = "Hello World"
	p = "Lorem ipsum :)"
```
