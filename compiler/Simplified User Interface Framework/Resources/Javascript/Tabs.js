function selectTab(tabSelectorGroupId, tabSelectorId, tabContentGroupId, tabContentId) {
	console.log(tabSelectorGroupId + '   ' + tabSelectorId + '   ' + tabContentGroupId + '   ' + tabContentId);
	let i;

	const tabContentGroup = document.getElementById(tabContentGroupId);
	for (i = 0; i < tabContentGroup.children.length; i++) {
		tabContentGroup.children[i].style.display = "none";
	}

	const tabSelectorGroup = document.getElementById(tabSelectorGroupId);
	for (i = 0; i < tabSelectorGroup.children.length; i++) {
		const child = tabSelectorGroup.children[i];
		child.className = child.className.replace(" active", "");
	}

	document.getElementById(tabContentId).style.display = "block";
	document.getElementById(tabSelectorId).className += " active";
}