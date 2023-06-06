
async function switchTheme() {
    // retrieve the currently stored theme from localstorage (this may have a value, or be undefined)
    let currentTheme = localStorage.getItem('theme')

    // if the current theme is not 'undefined' and is set to dark, then we are setting the theme to 'light'
    if (currentTheme && currentTheme == 'light') {

        // update the localStorage to set the theme to light
        localStorage.setItem('theme', 'dark')

        // Send a POST request to the server to update the session to record the change in theme
        let result = await fetch('/api/Settings/SetTheme', {
            method: 'POST',
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({ theme: "dark" })
        })
        console.log(result)
        // perform the client-side change to the theme, by switching out the stylesheet (this is faster with caching enabled!)
        document.getElementById('themeStyle').setAttribute('href', '/css/DarkTheme.css')

    } else {
        // setting the theme to dark
        localStorage.setItem('theme', 'light')

        // sending a POST request with a query parameter - simpler than sending a post request with Body parameters
        let result = await fetch('/api/Settings/SetTheme', {
            method: 'POST',
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({theme: "light"})
        })
        console.log(result)

        document.getElementById('themeStyle').setAttribute('href', '/css/LightTheme.css')
    }
}