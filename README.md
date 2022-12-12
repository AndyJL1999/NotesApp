<h1>NotesApp</h1>

<h2>Keeping Notes</h2>
<p>
To continue improving at WPF, I'm now moving on to more robust applications. This note app is based off of evernote.
It has the bare-bones functionality that is expected of a note-taking app. You are capable of creating notebooks and adding notes to those notebooks.
There is a basic text-editor that allows you to type a note out, change the text's font-family, font-weight, and underline the text. There was also supposed
to be speech-to-text using azure but I decided not to go with it due to lacking a subscription. Trying to use that function will throw an error message.
To save your note you press the save button.
</p>

<img src="https://user-images.githubusercontent.com/88408654/197054156-f0074eb5-b2e3-4a5c-87d3-36b8dbe1c57f.PNG"/>

<h2>MVVM</h2>
<p>
I've recently started implementing MVVM and this app is one of those implementations. There are only 2 views with corresponding view models; the login and notes view.
</p>

<img src="https://user-images.githubusercontent.com/88408654/197054176-9702bf62-e546-4c34-ac5f-985f6a5c96a3.PNG"/>

<p>
To reduce the amount of code in the code-behind I've begun using commands. Some of these commands are the delete and edit commands which are both used in the context
menu of the notebokk items and note items. Simply right-click and you will be given an option to 'rename' or 'delete'; note that these options both show only in the notebook 
selection and only 'delete' shows for the note selection. Notes can be renamed from within the editor and pressing save. The NotesWindow still contains a heavy amount 
of code in it's code-behind, it containing much of the logic that is used for navigating the notes and notebooks along with the text-editor functions. 
I plan on cleaning it up at a later date.
</p>

<h2>Google Firebase Services</h2>
<p>
My data is currently being stored on the cloud using Google Firebase Services. My authentication, database, and storage are all provided by Firebase. While the
authentication and database are working well, calls to the storage seem to be a bit slow. This causes a delay when retrieving and displaying notes. The delay can last
from half a second to a full second, so its not too intrusive but I'd prefer it dodn't exist.
</p>

<h2>BONUS</h2>
<p>
A little bonus to add spice to my app would be a slide animation added to the notes panel. Whenever a notebook is first selected the notes panel will slide into view.
This only ever happens in the beginning. I decided it would be best not to make it a constant attraction.
</p>
