// Dynamic quiz-question / quiz-option builder used on Admin Quiz create/edit pages.
document.addEventListener('DOMContentLoaded', function () {
    var addQuestionBtn = document.getElementById('addQuestionBtn');
    var questionsContainer = document.getElementById('questionsContainer');

    if (addQuestionBtn && questionsContainer) {
        addQuestionBtn.addEventListener('click', function () {
            var index = questionsContainer.querySelectorAll('.question-block').length;
            var template = document.getElementById('questionTemplate').innerHTML.replace(/__index__/g, index);
            var wrapper = document.createElement('div');
            wrapper.innerHTML = template;
            questionsContainer.appendChild(wrapper.firstElementChild);
        });

        questionsContainer.addEventListener('click', function (e) {
            if (e.target.classList.contains('remove-question-btn')) {
                e.target.closest('.question-block').remove();
            }
        });
    }

    // Auto-dismiss alerts after 5 seconds
    document.querySelectorAll('.alert').forEach(function (alertEl) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alertEl);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });
});
