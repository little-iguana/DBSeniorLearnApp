// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


const _checkbox = document.getElementById("checkbox");
_checkbox.addEventListener("change", () => {
	document.body.classList.toggle("dark");
});
