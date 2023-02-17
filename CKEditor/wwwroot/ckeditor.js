export function setup(id, dotNetReference) {
  ClassicEditor
    .create(document.querySelector('#ckeditor-' + id), {

      
      language: 'de',
      licenseKey: '',


    })
    .then(editor => {
      window.editor = editor;
      editor.model.document.on('change:data', () => {
        let data = editor.getData();

        const el = document.createElement('div');
        el.innerHTML = data;
        if (el.innerText.trim() == '')
          data = "";



        dotNetReference.invokeMethodAsync('EditorDataChanged', data);
      });
    })
      .catch(error => {
          console.error('Oops, something went wrong!');
          console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
          console.warn('Build id: n48v085f21si-dqhb6duovqri');
          console.error(error);
      });
}

export function destroy(id) {
  var editors = document.querySelectorAll('.ck-editor__editable');
  for (var i = 0; i < editors.length; i++) {
      if (editors[i].ckeditorInstance != null && editors[i].ckeditorInstance.sourceElement.id == "ckeditor-" + id) {
      editors[i].ckeditorInstance.destroy();
    }
  }
}

export function update(id, content) {
  var editors = document.querySelectorAll('.ck-editor__editable');
  for (var i = 0; i < editors.length; i++) {
      if (editors[i].ckeditorInstance != null && editors[i].ckeditorInstance.sourceElement.id == "ckeditor-" + id) {
      editors[i].ckeditorInstance.setData(content == null ? '' : content);
    }
  }
}