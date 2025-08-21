export function setup(id, dotNetReference) {

    ClassicEditor
        .create(document.querySelector('#ckeditor-' + id), {
            language: 'en',
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

                var editorContent = editor.getData();

                if (editorContent.length == 0) {
                    dotNetReference.invokeMethodAsync('EditorDataChanged', "", true);
                } else {
                    // Split content into chunks (e.g., 1 KB each)
                    const chunkSize = 100000;
                    for (let i = 0; i < editorContent.length; i += chunkSize) {
                        const chunk = editorContent.slice(i, i + chunkSize);
                        // Call JavaScript function to send each chunk
                        var isLastTransfer = editorContent.length < i + chunkSize;
                        const myObject = {
                            contentLength: editorContent.length,
                            i: i,
                            chunkSize: chunkSize,
                            isLastTransfer: isLastTransfer,
                            chunk: chunk
                        };
                        console.table(myObject);

                        dotNetReference.invokeMethodAsync('EditorDataChanged', chunk, isLastTransfer);
                    }

                }

            });
        })
        .catch(error => {
            console.error('Oops, something went wrong!');
            console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
            console.warn('Build id: n48v085f21si-dqhb6duovqri');
            console.error(error);
        });
}

export function update(id, data) {
    var editors = document.querySelectorAll('.ck-editor__editable');
    for (var i = 0; i < editors.length; i++) {
        if (editors[i].ckeditorInstance != null && editors[i].ckeditorInstance.sourceElement.id == "ckeditor-" + id) {
            editors[i].ckeditorInstance.setData(data);
        }
    }
}

export function destroy(id) {
    var editors = document.querySelectorAll('.ck-editor__editable');
    for (var i = 0; i < editors.length; i++) {
        if (editors[i].ckeditorInstance != null && editors[i].ckeditorInstance.sourceElement.id == "ckeditor-" + id) {
            editors[i].ckeditorInstance.destroy();
        }
    }
}

export function setReadonly(id, readonly) {
    var editors = document.querySelectorAll('.ck-editor__editable');
    for (var i = 0; i < editors.length; i++) {
        if (editors[i].ckeditorInstance != null && editors[i].ckeditorInstance.sourceElement.id == "ckeditor-" + id) {
            if (readonly) {
                editors[i].ckeditorInstance.enableReadOnlyMode("ckeditor-" + id);
            } else {
                editors[i].ckeditorInstance.disableReadOnlyMode("ckeditor-" + id);
            }
        }
    }
}
