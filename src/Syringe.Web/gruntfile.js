module.exports = function (grunt) {
    grunt.initConfig({

        sass: {
            dist: {
                options: {
                    style: 'compressed'
                },
                files: {
                    'css/Syringe.css': 'css/Syringe.scss'
                }
            }
        },
        watch: {

            css: {
                files: ['scss/*.scss'],
                tasks: ['sass'],
                options: {
                    spawn: false
                }
            }
        }
    });

    grunt.registerTask("default", ["sass"]);

    grunt.loadNpmTasks("grunt-bower-task");
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-contrib-sass");
};