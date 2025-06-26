let user = null;
let studentID = user && user.role === 'student' ? user.id : null;
let selectedCourseID = null;
let currentCourseIsClosed = false;
let selectedDay = '';
function showSection(sectionToShow) {
    hideAllSections();
    sectionToShow.classList.remove("hidden");

    document.getElementById("showLunchMenuBtn").classList.add("hidden");
    document.getElementById("showTimetableBtn").classList.add("hidden");
    document.getElementById("showGradebookBtn").classList.add("hidden");
    document.getElementById("showCalendarBtn").classList.add("hidden");
    document.getElementById("showCourseBtn").classList.add("hidden");
    document.getElementById("showGalleryBtn").classList.add("hidden");
    document.getElementById("showStatsBtn").classList.add("hidden");
}
function hideAllSections() {
    const sections = [lunchMenuSection, timetableSection, gradebookSection, calendarSection, createCourseSection, gallerySection, statsSection];
    sections.forEach(section => section.classList.add("hidden"));
}

document.addEventListener("DOMContentLoaded", function () {
    const showLunchMenuBtn = document.getElementById("showLunchMenuBtn");
    const showTimetableBtn = document.getElementById("showTimetableBtn");
    const showGradebookBtn = document.getElementById("showGradebookBtn");
    const showCalendarBtn = document.getElementById("showCalendarBtn");
    const showCourseBtn = document.getElementById("showCourseBtn");
    const showGalleryBtn = document.getElementById("showGalleryBtn");
    const showStatsBtn = document.getElementById("showStatsBtn");

    const lunchMenuSection = document.getElementById("lunchMenuSection");
    const timetableSection = document.getElementById("timetableSection");
    const gradebookSection = document.getElementById("gradebookSection");
    const calendarSection = document.getElementById("calendarSection");
    const createCourseSection = document.getElementById("createCourseSection");
    const gallerySection = document.getElementById("gallerySection");
    const statsSection = document.getElementById("statsSection");

    showLunchMenuBtn.addEventListener("click", function () {
        showSection(lunchMenuSection);
    });

    showTimetableBtn.addEventListener("click", function () {
        showSection(timetableSection);
    });

    showGradebookBtn.addEventListener("click", function () {
        showSection(gradebookSection);
    });

    showCalendarBtn.addEventListener("click", function () {
        showSection(calendarSection);
    });

    showCourseBtn.addEventListener("click", function () {
        showSection(createCourseSection);
    });

    showGalleryBtn.addEventListener("click", function () {
        showSection(gallerySection);
    });

    showStatsBtn.addEventListener("click", function () {
        showSection(statsSection);
    });
});

function showSections() {
    document.getElementById('showCourseBtn').classList.remove('hidden');
    document.getElementById('calendar').classList.remove('hidden');
    document.getElementById('templateSection').classList.remove('hidden');
    document.getElementById('showGalleryBtn').classList.remove('hidden');
    document.getElementById('showStatsBtn').classList.remove('hidden');

    const images = document.querySelectorAll('#gallerySection img');

    images.forEach(image => {
        image.style.width = '120px';
        image.style.height = '120px';
        image.style.objectFit = 'cover';
        image.style.borderRadius = '8px';
        image.style.boxShadow = '0px 4px 6px rgba(0, 0, 0, 0.1)';
        image.style.transition = 'transform 0.3s ease'; 

        image.addEventListener('mouseover', () => {
            image.style.transform = 'scale(1.05)';
        });

        image.addEventListener('mouseout', () => {
            image.style.transform = 'scale(1)';
        });
    });
}

function goBack() {
    hideAllSections();
    document.getElementById('loginImage').classList.add('hidden');
    sessionStorage.setItem('loginImageHidden', 'true');
    window.location.href = 'index.html';
}

document.addEventListener("DOMContentLoaded", function () {
    if (sessionStorage.getItem('loginImageHidden') === 'true') {
        document.getElementById('loginImage').classList.add('hidden');
    }
});

function showRegister() {
    document.getElementById('login').classList.add('hidden');
    document.getElementById('register').classList.remove('hidden');
    document.getElementById('showRegisterBtn').classList.add('hidden');
}

function showLogin() {
    document.getElementById('register').classList.add('hidden');
    document.getElementById('login').classList.remove('hidden');
    document.getElementById('showRegisterBtn').classList.remove('hidden');
}

$.get('/User/CheckSession')
    .done(function (response) {
        if (response.userID !== -1) {
            const username = response.username;
            const role = response.role;
            user = { id: response.userID, role: role };

            document.getElementById('login').classList.add('hidden');
            document.getElementById('userInfo').innerText = `Bejelentkezve: ${username} (${role})`;
            document.getElementById('logoutBtn').classList.remove('hidden');
            document.getElementById('showRegisterBtn').classList.add('hidden');
            document.getElementById('showLunchMenuBtn').classList.remove('hidden');
            document.getElementById('showTimetableBtn').classList.remove('hidden');
            document.getElementById('showGradebookBtn').classList.remove('hidden');
            document.getElementById('showCalendarBtn').classList.remove('hidden');
            document.getElementById('showCourseBtn').classList.remove('hidden');
            document.getElementById('showGalleryBtn').classList.remove('hidden');
            document.getElementById('showStatsBtn').classList.remove('hidden');
            document.getElementById('loginImage').classList.add('hidden');
            showSections();
            if (role === 'student' || role === 'teacher') {
                document.getElementById('addTimetableEntryForm').classList.add('hidden');
                document.getElementById('createGrade').classList.add('hidden');
                document.getElementById('addEventForm').classList.add('hidden');
            } else {
                document.getElementById('addTimetableEntryForm').classList.remove('hidden');
                document.getElementById('createGrade').classList.remove('hidden');
                document.getElementById('addEventForm').classList.remove('hidden');
            }
        } else {
            user = null;
            document.getElementById('login').classList.remove('hidden');
            document.getElementById('userInfo').innerText = '';
            document.getElementById('logoutBtn').classList.add('hidden');
            document.getElementById('showRegisterBtn').classList.remove('hidden');
            document.getElementById('showLunchMenuBtn').classList.add('hidden');
            document.getElementById('showTimetableBtn').classList.add('hidden');
            document.getElementById('showGradebookBtn').classList.add('hidden');
            document.getElementById('showCalendarBtn').classList.add('hidden');
            document.getElementById('showCourseBtn').classList.add('hidden');
            document.getElementById('showGalleryBtn').classList.add('hidden');
            document.getElementById('showStatsBtn').classList.add('hidden');
            document.getElementById('loginImage').classList.remove('hidden');
            hideAllSections();
        }
    })
    .fail(function () {
        console.error('Session check failed.');
    });

function register() {
    const username = $('#regUsername').val();
    const password = $('#regPassword').val();
    const role = $('#role').val();

    $.post('/User/Create', { username, password, role })
        .done(function (response) {
            alert(response);
            $("#regUsername").val('');
            $("#regPassword").val('');
            showLogin();
        })
        .fail(function (xhr) {
            alert('Hiba a regisztráció során: ' + xhr.responseText);
        });
}

function login() {
    const username = $('#username').val();
    const password = $('#password').val();
    const role = $('#role').val();

    $.post('/User/Login', { username, password, role })
        .done(function (response) {
            alert(response.message);
            $("#username").val('');
            $("#password").val('');
            const role = response.role;
            user = { id: response.userID || null, name: username || '', role: role };
            studentID = user.role === 'student' ? user.id : null;
            document.getElementById('login').classList.add('hidden');
            document.getElementById('userInfo').innerText = `Bejelentkezve: ${username}(${role})`;
            document.getElementById('logoutBtn').classList.remove('hidden');
            document.getElementById('showRegisterBtn').classList.add('hidden');
            document.getElementById('loginImage').classList.add('hidden');
            document.getElementById('showLunchMenuBtn').classList.remove('hidden');
            document.getElementById('showTimetableBtn').classList.remove('hidden');
            document.getElementById('showGradebookBtn').classList.remove('hidden');
            document.getElementById('showCalendarBtn').classList.remove('hidden');
            document.getElementById('showCourseBtn').classList.remove('hidden');
            document.getElementById('showGalleryBtn').classList.remove('hidden');
            document.getElementById('showStatsBtn').classList.remove('hidden');
            showSections();
            if (user) {
                loadTimetable();
                loadEvents();
                loadWeeklyMenu();
                loadDropdowns();
                loadCourses();
                loadStudentStatistics();
                loadTeacherStatistics();
                loadAdminStatistics();
                if (studentID) {
                    loadAverageGrade();
                }
            }
        })
        .fail(function (xhr) {
            alert('Hibás felhasználónév vagy jelszó!');
        });
}

function logout() {
    $.post('/User/Logout')
        .done(function (response) {
            user = null;
            document.getElementById('login').classList.remove('hidden');
            document.getElementById('userInfo').innerText = '';
            document.getElementById('logoutBtn').classList.add('hidden');
            document.getElementById('showRegisterBtn').classList.remove('hidden');
            document.getElementById('username').value = '';
            document.getElementById('password').value = '';
            document.getElementById('role').selectedIndex = 0;
            document.getElementById('timetableSection').classList.add('hidden');
            document.getElementById('gradebookSection').classList.add('hidden');
            document.getElementById('calendarSection').classList.add('hidden');
            document.getElementById('lunchMenuSection').classList.add('hidden');
            document.getElementById('createCourseSection').classList.add('hidden');
            document.getElementById('gallerySection').classList.add('hidden');
            document.getElementById('showLunchMenuBtn').classList.add('hidden');
            document.getElementById('showTimetableBtn').classList.add('hidden');
            document.getElementById('showGradebookBtn').classList.add('hidden');
            document.getElementById('showCalendarBtn').classList.add('hidden');
            document.getElementById('showCourseBtn').classList.add('hidden');
            document.getElementById('showGalleryBtn').classList.add('hidden');
            document.getElementById('showStatsBtn').classList.add('hidden');
            document.getElementById('loginImage').classList.remove('hidden');
        })
        .fail(function (xhr) {
            alert('Hiba a kijelentkezés során: ' + xhr.responseText);
        });
}

function loadCourses() {
    $.get('/Course/GetCoursesForUser')
        .done(function (courses) {
            const courseList = $('#courseList');
            courseList.empty();

            if (courses.length === 0) {
                courseList.append('<p>Nincsenek kurzusok.</p>');
                return;
            }

            courses.forEach(course => {
                const courseItem = $(`
                    <div class="course-item" data-id="${course.courseID}">
                        <h3>${course.name}</h3>
                        <p>Tanár ID: ${course.teacherID}</p>
                        <button class="addMaterialBtn">Add Material</button>
                    </div>
                `);
                courseList.append(courseItem);
            });
        })
        .fail(function () {
            alert('Kurzusok betöltése sikertelen.');
        });
}

$('#createCourseBtn').click(function () {
    const name = $('#newCourseName').val().trim();
    const isVisible = $('#newCourseVisible').is(':checked');

    if (!name) {
        alert('Adj meg egy kurzusnevet!');
        return;
    }

    $.post('/Course/CreateCourse', { name: name, isVisible: isVisible })
        .done(function (response) {
            alert('Kurzus sikeresen létrehozva.');
            $('#newCourseName').val('');
            loadCourses();
            selectedCourseID = response.courseID;
        })
        .fail(function () {
            alert('Kurzus létrehozása sikertelen.');
        });
});
function loadMaterials(courseID) {
    $.get('/Course/GetMaterials', { courseID: courseID })
        .done(function (materials) {
            const list = $('#materialsList');
            list.empty();
            materials.forEach(m => list.append(`<li>${m.title}: ${m.content}</li>`));
        })
        .fail(() => alert('Tananyagok betöltése sikertelen.'));
}

function loadTests(courseID) {
    $.get('/Course/GetTests', { courseID: courseID })
        .done(function (tests) {
            const list = $('#testsList');
            list.empty();
            tests.forEach(t => list.append(`<li>${t.testName} - Határidő: ${new Date(t.deadline).toLocaleDateString()}</li>`));
        })
        .fail(() => alert('Tesztek betöltése sikertelen.'));
}

function loadAssignments(courseID) {
    $.get('/Course/GetAssignments', { courseID: courseID })
        .done(function (assignments) {
            const list = $('#assignmentsList');
            list.empty();
            assignments.forEach(a => list.append(`<li>${a.title} - Határidő: ${new Date(a.deadline).toLocaleDateString()}</li>`));
        })
        .fail(() => alert('Beadandók betöltése sikertelen.'));
}

$('#addMaterialBtn').click(function () {
    if (currentCourseIsClosed) {
        alert("A tárgy le van zárva, nem lehet tananyagot hozzáadni.");
        return;
    }
    const title = $('#materialTitle').val();
    const content = $('#materialContent').val();
    if (!title || !content) {
        alert('Add meg a tananyag címét és tartalmát!');
        return;
    }
    $.post('/Course/AddMaterial', { courseID: selectedCourseID, title: title, content: content })
        .done(() => {
            alert('Tananyag hozzáadva.');
            $('#materialTitle').val('');
            $('#materialContent').val('');
            loadMaterials(selectedCourseID);
        })
        .fail(() => alert('Tananyag hozzáadása sikertelen.'));
});

$('#addTestBtn').click(function () {
    if (currentCourseIsClosed) {
        alert("A tárgy le van zárva, nem lehet tananyagot hozzáadni.");
        return;
    }
    const testName = $('#testName').val();
    const description = $('#testDescription').val();
    const deadline = $('#testDeadline').val();
    if (!testName || !description || !deadline) {
        alert('Tölts ki minden teszt mezőt!');
        return;
    }
    $.post('/Course/CreateTest', { courseID: selectedCourseID, testName, description, deadline })
        .done(() => {
            alert('Teszt létrehozva.');
            $('#testName').val('');
            $('#testDescription').val('');
            $('#testDeadline').val('');
            loadTests(selectedCourseID);
        })
        .fail(() => alert('Teszt létrehozása sikertelen.'));
});

$('#addAssignmentBtn').click(function () {
    if (currentCourseIsClosed) {
        alert("A tárgy le van zárva, nem lehet tananyagot hozzáadni.");
        return;
    }
    const title = $('#assignmentTitle').val();
    const description = $('#assignmentDescription').val();
    const deadline = $('#assignmentDeadline').val();
    if (!title || !description || !deadline) {
        alert('Tölts ki minden beadandó mezőt!');
        return;
    }
    $.post('/Course/CreateAssignment', { courseID: selectedCourseID, title, description, deadline })
        .done(() => {
            alert('Beadandó létrehozva.');
            $('#assignmentTitle').val('');
            $('#assignmentDescription').val('');
            $('#assignmentDeadline').val('');
            loadAssignments(selectedCourseID);
        })
        .fail(() => alert('Beadandó létrehozása sikertelen.'));
});

function loadMenuByDay(day) {
    selectedDay = day;
    if (!day) {
        const list = $('#lunchMenuList');
        list.empty().append('<li>Kérlek válassz ki egy napot!</li>');
        return;
    }

    $.get('/Lunch/GetMenuByDay', { day: day })
        .done(function (menu) {
            const list = $('#lunchMenuList');
            list.empty();

            if (menu.length === 0) {
                list.append('<li>Nincs étel erre a napra.</li>');
                return;
            }

            const userID = 1;
            $.get('/Lunch/GetUserLunchSignups', { userID: userID })
                .done(function (userSignups) {
                    menu.forEach(item => {
                        const listItem = $(`
                <li>
                    ${item.meal} 
                    <button class="signupLunchBtn" data-meal="${item.meal}" data-day="${day}">Jelentkezés</button>
                </li>
            `);

                        const button = listItem.find('.signupLunchBtn');

                        if (userSignups.some(signup => signup.meal === item.meal && signup.day === day)) {
                            button.text('Lejelentkezés');
                            button.removeClass('signupLunchBtn').addClass('cancelSignupLunchBtn');
                            void button[0].offsetWidth;
                            listItem.append('<span class="checkmark">&#10003;</span>');
                        } else {
                            button.removeClass('cancelSignupLunchBtn').addClass('signupLunchBtn');
                        }

                        list.append(listItem);
                    });
                })
                .fail(function () {
                    console.error('Nem sikerült lekérdezni a felhasználó jelentkezéseit.');
                });
        });
}
function loadWeeklyMenu() {
    $.get('/Lunch/GetWeeklyMenu')
        .done(function (menu) {
            const list = $('#lunchMenuList');
            list.empty();
            if (menu.length === 0) {
                list.append('<li>Nincs étlap adat.</li>');
                return;
            }
            let currentDay = '';
            menu.forEach(item => {
                if (currentDay !== item.day) {
                    currentDay = item.day;
                    list.append(`<li><strong>${currentDay}</strong></li>`);
                }
                list.append(`<li> - ${item.meal}</li>`);
            });
        })
        .fail(function () {
            alert('Heti étlap betöltése sikertelen.');
        });
}

$('#addLunchBtn').click(function () {
    const day = $('#newLunchDay').val();
    const meal = $('#newLunchMeal').val().trim();

    if (!meal) {
        alert('Adj meg egy étel nevet!');
        return;
    }

    $.post('/Lunch/CreateLunch', { day: day, meal: meal })
        .done(function () {
            alert('Étel hozzáadva!');
            $('#newLunchMeal').val('');
            loadMenuByDay(day);
            $('#menuDaySelect').val(day);
        })
        .fail(function () {
            alert('Étel hozzáadása sikertelen.');
        });
});

$('#loadMenuByDayBtn').click(function () {
    const day = $('#menuDaySelect').val();
    loadMenuByDay(day);
});

$('#showDailyMenuBtn').click(function () {
    loadMenuByDay(today);
});

$('#showWeeklyMenuBtn').click(function () {
    loadWeeklyMenu();
});

$('#menuDaySelect').change(function () {
    const selectedDay = $(this).val();
    loadMenuByDay(selectedDay);
});

$('#insertDefaultMenuBtn').click(function () {
    $.post('/Lunch/InsertDefaultWeeklyMenu')
        .done(function () {
            alert('Alapértelmezett heti menü beszúrva.');
            loadWeeklyMenu();
        })
        .fail(function () {
            alert('Menü beszúrása sikertelen.');
        });
});

$(document).on('click', '.signupLunchBtn', function () {
    const meal = $(this).data('meal');
    const day = $(this).data('day');
    const userID = 1;
    const button = $(this);

    if (!meal || !day) {
        alert('Hibás adatok! Kérlek válassz ételt és napot!');
        return;
    }

    $.post('/Lunch/SignupLunch', { userID: userID, day: day, meal: meal })
        .done(function () {
            alert('Sikeresen jelentkeztél az ebédre!');

            //button = $(`[data-meal="${meal}"][data-day="${day}"]`);
            button.text('Lejelentkezés');
            button.addClass('cancelSignupLunchBtn');
            button.removeClass('signupLunchBtn');

            const listItem = button.closest('li');
            listItem.append('<span class="checkmark">&#10003;</span>');
        })
        .fail(function () {
            alert('Jelentkezés nem sikerült. Kérlek próbáld újra!');
        });
});

$(document).on('click', '.cancelSignupLunchBtn', function () {
    const meal = $(this).data('meal');
    const day = $(this).data('day');
    const userID = 1;
    const button = $(this);

    $.post('/Lunch/CancelLunchSignup', { userID: userID, day: day, meal: meal })
        .done(function () {
            alert('Sikeresen lejelentkeztél az ebédről!');

            button.text('Jelentkezés');
            button.addClass('signupLunchBtn');
            button.removeClass('cancelSignupLunchBtn');

            const listItem = button.closest('li');
            listItem.find('.checkmark').remove();
        })
        .fail(function () {
            alert('Lejelentkezés nem sikerült. Kérlek próbáld újra!');
        });
});

function loadDropdowns() {
    $.get('/Timetable/GetSubjects', function (subjects) {
        const dropdown = $('#subjectSelect');
        dropdown.empty().append('<option value="">Válassz tantárgyat</option>');
        subjects.forEach(s => {
            dropdown.append(`<option value="${s.subjectID}">${s.subjectName}</option>`);
        });
    });

    $.get('/Timetable/GetRooms', function (rooms) {
        const dropdown = $('#roomSelect');
        dropdown.empty().append('<option value="">Válassz termet</option>');
        rooms.forEach(r => {
            dropdown.append(`<option value="${r.roomID}">${r.roomName}</option>`);
        });
    });

    $.get('/Timetable/GetClasses', function (classes) {
        const dropdown = $('#classSelect');
        dropdown.empty().append('<option value="">Válassz osztályt</option>');
        classes.forEach(c => {
            dropdown.append(`<option value="${c.classID}">${c.className}</option>`);
        });
    });
}
function loadTimetable() {
    $.get('/Timetable/GetTimetable').done(function (entries) {
        const $tbody = $('#timetableTable tbody').empty();
        if (entries.length === 0) {
            $tbody.append('<tr><td colspan="6">Nincs órarend bejegyzés</td></tr>');
            return;
        }

        entries.forEach(e => {
            $tbody.append(`
                    <tr>
                        <td>${e.day}</td>
                        <td>${e.hour}</td>
                        <td>${e.subjectID}</td>
                        <td>${e.roomID}</td>
                        <td>${e.teacherID}</td>
                        <td>${e.classID}</td>
                    </tr>
                `);
        });
    });
}

$('#addTimetableEntryForm').on('submit', function (e) {
    e.preventDefault();
    const day = $('#daySelect').val();
    const hour = $('#hourInput').val();
    const subjectID = parseInt($('#subjectSelect').val());
    const roomID = parseInt($('#roomSelect').val());
    const teacherID = parseInt($('#teacherIDInput').val());
    const classID = parseInt($('#classSelect').val());

    if (!day || !hour || !subjectID || !roomID || !teacherID || !classID) {
        $('#message').text('Kérlek, tölts ki minden mezőt!').css('color', 'red');
        return;
    }

    $.post('/Timetable/CreateTimetableEntry', { day, hour, subjectID, roomID, teacherID, classID })
        .done(function (res) {
            $('#message').text(res).css('color', 'green');
            $('#addTimetableEntryForm')[0].reset();
            loadTimetable();
        })
        .fail(function (xhr) {
            $('#message').text('Hiba: ' + xhr.responseText).css('color', 'red');
        });
});

$(document).ready(function () {
    $.post('/Timetable/SeedSubjects')
        .done(function (res) {
            $('#seedStatus').text(res).css('color', 'green');
            loadDropdowns();
        })
        .fail(function (xhr) {
            $('#seedStatus').text('Hiba: ' + xhr.responseText).css('color', 'red');
        });

    $.post('/Timetable/SeedRooms')
        .done(function (res) {
            $('#roomSeedStatus').text(res).css('color', 'green');
            loadDropdowns();
        })
        .fail(function (xhr) {
            $('#roomSeedStatus').text('Hiba: ' + xhr.responseText).css('color', 'red');
        });

    $.post('/Timetable/SeedClasses')
        .done(function (res) {
            $('#classSeedStatus').text(res).css('color', 'green');
            loadDropdowns();
        })
        .fail(function (xhr) {
            $('#classSeedStatus').text('Hiba: ' + xhr.responseText).css('color', 'red');
        });

    $.post('/Lunch/InsertDefaultWeeklyMenu')
        .done(function () {
            loadWeeklyMenu();
        })
        .fail(function () {
            console.warn('Az alapértelmezett heti menü már létezik vagy hiba történt.');
            loadWeeklyMenu();
        });

    $.post('/Statistics/SeedStudents')
        .done(function (data) {
        })
        .fail(function (xhr) {
            console.error('Error seeding students:', xhr);
            alert('Hiba történt a diákok feltöltése közben.');
        });

    $.post('/Statistics/SeedTeachers')
        .done(function (data) {
        })
        .fail(function (xhr) {
            console.error('Error seeding teachers:', xhr);
            alert('Hiba történt a tanárok feltöltése közben.');
        });

    $.post('/Statistics/SeedSubjectTeachers')
        .done(function (data) {
        })
        .fail(function (xhr) {
            console.error('Error seeding subject-teacher assignments:', xhr);
            alert('Hiba történt a tantárgy-tanár kapcsolatok feltöltése közben.');
        });
});

function loadGrades() {
    const studentID = parseInt($("#studentIDInput").val());
    const subjectID = parseInt($("#subjectIDInput").val());

    if (isNaN(studentID) || isNaN(subjectID)) {
        alert("Kérlek add meg a diák és tantárgy ID-t.");
        return;
    }

    $.get(`/Gradebook/GetGrades?studentID=${studentID}&subjectID=${subjectID}`, function (data) {
        const tbody = $("#gradesTable tbody");
        tbody.empty();
        data.forEach(g => {
            tbody.append(`<tr>
                    <td>${g.gradeID}</td>
                    <td>${g.grade}</td>
                    <td>${new Date(g.date).toLocaleDateString()}</td>
                    <td><button class="deleteGradeBtn" data-id="${g.gradeID}">Törlés</button></td>
                </tr>`);
        });
    });
}

function loadAbsences(studentID) {
    $.get(`/Gradebook/GetAbsences?studentId=${studentID}`, function (data) {
        const tbody = $("#absencesTable tbody");
        tbody.empty();
        data.forEach(a => {
            const justifiedText = a.isJustified ? "Igen" : "Nem";
            const justifyButtonText = a.isJustified ? "Hiányzás nincs igazolva" : "Igazolás";
            const buttonClass = a.isJustified ? "removeJustificationBtn" : "justifyAbsenceBtn";

            tbody.append(`<tr>
                    <td>${a.absenceID}</td>
                    <td>${new Date(a.date).toLocaleDateString()}</td>
                    <td>${justifiedText}</td>
                    <td>
                        <button class="deleteAbsenceBtn" data-id="${a.absenceID}">Törlés</button>
                        <button class="${buttonClass}" data-id="${a.absenceID}" data-isJustified="${a.isJustified}">${justifyButtonText}</button>
                    </td>
                </tr>`);
        });
    });
}

$("#loadGradesBtn").click(function () {
    loadGrades();
});

$("#loadAbsencesBtn").click(function () {
    const studentID = $("#studentIDInput").val();
    loadAbsences(studentID);
});

$("#addGradeBtn").click(function () {
    const studentID = $("#studentIDInput").val();
    const subjectID = $("#subjectIDInput").val();
    const grade = $("#newGradeInput").val();
    const date = $("#newGradeDate").val();

    if (!date) {
        alert("Adj meg egy dátumot!");
        return;
    }

    $.post("/Gradebook/AddGrade", { studentID, subjectID, grade, date })
        .done(() => {
            alert("Érdemjegy hozzáadva");
            loadGrades(studentID, subjectID);
            $("#newGradeInput").val('');
            $("#newGradeDate").val('');
        })
        .fail(() => alert("Hiba történt az érdemjegy hozzáadásakor"));
});

$("#addAbsenceBtn").click(function () {
    const studentID = $("#studentIDInput").val();
    const date = $("#newAbsenceDate").val();

    if (!date) {
        alert("Adj meg egy dátumot!");
        return;
    }

    $.post("/Gradebook/AddAbsence", { studentID, date })
        .done(() => {
            alert("Hiányzás hozzáadva");
            loadAbsences(studentID);
            $("#newAbsenceDate").val('');
        })
        .fail(() => alert("Hiba történt a hiányzás hozzáadásakor"));
});

$("#gradesTable").on("click", ".deleteGradeBtn", function () {
    const gradeId = $(this).data("id");
    $.post("/Gradebook/DeleteGrade", { gradeId })
        .done(() => {
            alert("Érdemjegy törölve");
            $("#loadGradesBtn").click();
        })
        .fail(() => alert("Nem sikerült törölni az érdemjegyet"));
});

$("#absencesTable").on("click", ".deleteAbsenceBtn", function () {
    const absenceID = $(this).data("id");
    $.post("/Gradebook/DeleteAbsence", { absenceID })
        .done(() => {
            alert("Hiányzás törölve");
            $("#loadAbsencesBtn").click();
        })
        .fail(() => alert("Nem sikerült törölni a hiányzást"));
});

$("#absencesTable").on("click", ".justifyAbsenceBtn", function () {
    const absenceID = $(this).data("id");
    const button = $(this);

    $.post("/Gradebook/JustifyAbsence", { absenceID })
        .done(() => {
            button.text("Hiányzás nincs igazolva");
            button.data("isJustified", true);
            button.removeClass("btn-unjustified").addClass("btn-justified");
            alert("Hiányzás igazolva");
            $("#loadAbsencesBtn").click();
        })
        .fail(() => alert("Nem sikerült igazolni a hiányzást"));
});

$("#absencesTable").on("click", ".removeJustificationBtn", function () {
    const absenceID = $(this).data("id");
    const button = $(this);

    $.post("/Gradebook/RemoveJustification", { absenceID })
        .done(() => {
            button.text("Igazolás");
            button.data("isJustified", false);
            button.removeClass("btn-justified").addClass("btn-unjustified");
            alert("Hiányzás nincs igazolva");
            $("#loadAbsencesBtn").click();
        })
        .fail(() => alert("Nem sikerült eltávolítani az igazolást"));
});

function loadSubjectsForGradeClose() {
    $.get('/Timetable/GetSubjects', function (subjects) {
        const select = $('#gradeCloseSubjectSelect');
        select.empty();
        select.append('<option value="">--Válassz tantárgyat--</option>');
        subjects.forEach(subject => {
            select.append(`<option value="${subject.subjectID}">${subject.subjectName}</option>`);
        });
    });
}

$(document).ready(function () {
    loadSubjectsForGradeClose();
});

$('#closeGradesBtn').click(function () {
    const subjectID = parseInt($('#gradeCloseSubjectSelect').val());
    const isClosed = $('#isClosedCheckbox').is(':checked');

    if (!subjectID) {
        alert('Nincs kiválasztva tantárgy!');
        return;
    }

    $.post('/Gradebook/SetSubjectClosed', { subjectId: subjectID, isClosed: isClosed })
        .done(function (response) {
            alert(response);
        })
        .fail(function (xhr) {
            alert('Hiba történt: ' + xhr.responseText);
        });
});
function loadEvents() {
    fetch(`Calendar/GetEvents`)
        .then(res => res.json())
        .then(events => {
            const eventsList = document.getElementById('eventsList');
            const type = document.getElementById('eventType').value;
            eventsList.innerHTML = '';

            events.forEach(event => {
                const li = document.createElement('li');
                li.textContent = `${event.date.slice(0, 10)} - [${event.type}] ${event.title}: ${event.description}`;

                if (user && user.role === 'admin') {
                    const deleteBtn = document.createElement('button');
                    deleteBtn.textContent = 'Törlés';
                    deleteBtn.style.marginLeft = '10px';
                    deleteBtn.onclick = () => deleteEvent(event.eventID);
                    li.appendChild(deleteBtn);
                }

                eventsList.appendChild(li);
            });
        })
        .catch(err => console.error('Hiba az események betöltésekor:', err));
}

function deleteEvent(eventId) {
    const formData = new FormData();
    formData.append('eventId', eventId);

    fetch(`Calendar/DeleteEvent`, {
        method: 'POST',
        body: formData
    })
        .then(res => {
            if (!res.ok) throw new Error('Nem sikerült törölni az eseményt');
            return res.text();
        })
        .then(msg => {
            alert(msg);
            loadEvents();
        })
        .catch(err => alert(err.message));
}

document.getElementById('addEventForm').addEventListener('submit', function (e) {
    e.preventDefault();

    if (user && user.role !== 'admin') {
        alert('Csak adminisztrátorok adhatnak hozzá eseményeket!');
        return;
    }

    const title = document.getElementById('eventTitle').value.trim();
    const description = document.getElementById('eventDescription').value.trim();
    const date = document.getElementById('eventDate').value;
    const type = document.getElementById('eventType').value;

    if (!title || !description || !date) {
        alert('Kérlek töltsd ki az összes mezőt!');
        return;
    }

    const formData = new FormData();
    formData.append('title', title);
    formData.append('description', description);
    formData.append('date', date);
    formData.append('type', type);

    fetch(`Calendar/AddEvent`, {
        method: 'POST',
        body: formData
    })
        .then(res => {
            if (!res.ok) throw new Error('Nem sikerült hozzáadni az eseményt');
            return res.text();
        })
        .then(msg => {
            alert(msg);
            this.reset();
            loadEvents();
        })
        .catch(err => alert(err.message));
});

function drawStudentBarChart(grades) {
    const canvas = document.getElementById('gradesChart');
    const ctx = canvas.getContext('2d');

    const subjects = grades.map(grade => `Subject ${grade.subjectID}`);
    const gradesValues = grades.map(grade => grade.grade);

    const maxGrade = Math.max(...gradesValues);

    const barWidth = canvas.width / subjects.length;
    const scale = canvas.height / (maxGrade + 2);

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = 'blue';

    subjects.forEach((subject, index) => {
        const x = index * barWidth + 20;
        const y = canvas.height - gradesValues[index] * scale;
        const barHeight = gradesValues[index] * scale;

        ctx.fillRect(x, y, barWidth - 10, barHeight);
        ctx.fillText(subject, x + 10, canvas.height - 5);
    });

    ctx.fillStyle = 'black';
    ctx.font = '16px Arial';
    ctx.fillText('Grades by Subject', 10, 20);
}
function loadStudentStatistics(studentID) {
    $.get(`/Statistics/GetStudentStatistics`, { studentID: studentID })
        .done(data => {
            const { grades, absences } = data;
            const statisticsSection = document.getElementById('statsSection');
            statisticsSection.innerHTML = '';

            const gradesList = document.createElement('ul');
            gradesList.innerHTML = '<h3>Grades</h3>';
            grades.forEach(grade => {
                const li = document.createElement('li');
                li.textContent = `Subject ${grade.subjectID}: Grade ${grade.grade} - Date: ${grade.date}`;
                gradesList.appendChild(li);
            });

            const absencesList = document.createElement('ul');
            absencesList.innerHTML = '<h3>Absences</h3>';
            absences.forEach(absence => {
                const li = document.createElement('li');
                li.textContent = `Date: ${absence.date} - Justified: ${absence.isJustified ? 'Yes' : 'No'}`;
                absencesList.appendChild(li);
            });

            statisticsSection.appendChild(gradesList);
            statisticsSection.appendChild(absencesList);

            drawStudentBarChart(grades);
        })
        .fail(err => {
            console.error('Error loading student statistics:', err);
            alert('Hiba történt a statisztikák betöltése közben.');
        });
}

function drawTeacherBarChart(studentStats) {
    const canvas = document.getElementById('gradesChart');
    const ctx = canvas.getContext('2d');

    const students = studentStats.map(stat => stat.Student.StudentName);
    const studentGrades = studentStats.map(stat => {
        return stat.Grades.reduce((total, grade) => total + grade.grade, 0) / stat.Grades.length;
    });

    const maxGrade = Math.max(...studentGrades);

    const barWidth = canvas.width / students.length;
    const scale = canvas.height / (maxGrade + 2);

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = 'green';

    students.forEach((student, index) => {
        const x = index * barWidth + 20;
        const y = canvas.height - studentGrades[index] * scale;
        const barHeight = studentGrades[index] * scale;

        ctx.fillRect(x, y, barWidth - 10, barHeight);
        ctx.fillText(student, x + 10, canvas.height - 5);
    });

    ctx.fillStyle = 'black';
    ctx.font = '16px Arial';
    ctx.fillText('Average Grades of Students', 10, 20);
}

function loadTeacherStatistics(teacherID) {
    $.get(`/Statistics/GetTeacherStatistics`, { teacherID: teacherID })
        .done(data => {
            const statisticsSection = document.getElementById('statsSection');
            statisticsSection.innerHTML = '';

            data.forEach(studentStat => {
                const studentDiv = document.createElement('div');
                studentDiv.innerHTML = `<h3>Student: ${studentStat.Student.StudentName}</h3>`;

                const gradesList = document.createElement('ul');
                gradesList.innerHTML = '<h4>Grades</h4>';
                studentStat.Grades.forEach(grade => {
                    const li = document.createElement('li');
                    li.textContent = `Subject ${grade.subjectID}: Grade ${grade.grade} - Date: ${grade.date}`;
                    gradesList.appendChild(li);
                });

                const absencesList = document.createElement('ul');
                absencesList.innerHTML = '<h4>Absences</h4>';
                studentStat.Absences.forEach(absence => {
                    const li = document.createElement('li');
                    li.textContent = `Date: ${absence.date} - Justified: ${absence.isJustified ? 'Yes' : 'No'}`;
                    absencesList.appendChild(li);
                });

                studentDiv.appendChild(gradesList);
                studentDiv.appendChild(absencesList);
                statisticsSection.appendChild(studentDiv);
            });
            drawTeacherBarChart(data);
        })
        .fail(err => {
            console.error('Error loading teacher statistics:', err);
        });
}

function drawAdminBarChart(stats) {
    const canvas = document.getElementById('gradesChart');
    const ctx = canvas.getContext('2d');

    const categories = ['Students', 'Teachers', 'Classes'];
    const values = [stats.students.length, stats.teachers.length, stats.classes.length];

    const maxCount = Math.max(...values);

    const barWidth = canvas.width / categories.length;
    const scale = canvas.height / (maxCount + 2);

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = 'orange';

    categories.forEach((category, index) => {
        const x = index * barWidth + 20;
        const y = canvas.height - values[index] * scale;
        const barHeight = values[index] * scale;

        ctx.fillRect(x, y, barWidth - 10, barHeight);
        ctx.fillText(category, x + 10, canvas.height - 5);
    });

    ctx.fillStyle = 'black';
    ctx.font = '16px Arial';
    ctx.fillText('Total Statistics', 10, 20);
}
function loadAdminStatistics() {
    $.get(`/Statistics/GetAdminStatistics`)
        .done(data => {
            const statisticsSection = document.getElementById('statsSection');
            statisticsSection.innerHTML = '';

            const studentsList = document.createElement('ul');
            studentsList.innerHTML = '<h3>All Students</h3>';
            data.students.forEach(student => {
                const li = document.createElement('li');
                li.textContent = `${student.studentName} (ID: ${student.studentID})`;
                studentsList.appendChild(li);
            });

            const teachersList = document.createElement('ul');
            teachersList.innerHTML = '<h3>All Teachers</h3>';
            data.teachers.forEach(teacher => {
                const li = document.createElement('li');
                li.textContent = `${teacher.teacherName} (ID: ${teacher.teacherID})`;
                teachersList.appendChild(li);
            });

            const classesList = document.createElement('ul');
            classesList.innerHTML = '<h3>All Classes</h3>';
            data.classes.forEach(cls => {
                const li = document.createElement('li');
                li.textContent = `${cls.className} (ID: ${cls.classID})`;
                classesList.appendChild(li);
            });

            statisticsSection.appendChild(studentsList);
            statisticsSection.appendChild(teachersList);
            statisticsSection.appendChild(classesList);

            drawAdminBarChart(data);
        })
        .fail(err => {
            console.error('Error loading admin statistics:', err);
        });
}

function loadAverageGrade(studentID) {
    $.get(`/Statistics/GetGradesAverage`, { studentID: studentID })
        .done(data => {
            const avgGradeValue = document.getElementById('avgGradeValue');
            avgGradeValue.textContent = `Az átlagos érdemjegy: ${data.average}`;
        })
        .fail(err => {
            console.error('Error loading average grade:', err);
        });
}