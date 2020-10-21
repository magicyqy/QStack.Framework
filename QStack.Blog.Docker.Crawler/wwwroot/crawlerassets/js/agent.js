$(function () {
    new Vue({
        el: '#app',
        data: {
            page: 1,
            limit: 15,
            count: 0,
            items: [],
            baseUrl:'/crawler/api/v1.0/agents/'
        },
        mounted: function () {
            this.load();
        },
        methods: {
            load: function () {
                let that = this;
                let pagedQuery = getPagedQuery();
                http.get(this.baseUrl+`?${pagedQuery}`, function (result) {
                    that.page = result.page;
                    that.limit = result.pageSize;
                    that.count = result.totalCount;
                    that.items = [];
                    result.data.forEach(x => {
                        that.items.push(x);
                    });
                });
            },
            exit: function (event) {
                swal({
                    title: "Sure to stop this agent?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    let id = $(event.toElement).parent().parent().parent().attr('id');
                    debugger
                        http.put(this.baseUrl  + id + '/exit', null, function () {
                        swal('Success', 'Send signal success', "success");
                    }, function () {
                        swal('Error', 'Stop agent failed', "error");
                    });
                });
            },
            remove: function (event) {
                const that = this;
                swal({
                    title: "Sure to remove this agent?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    let id = $(event.toElement).parent().parent().parent().attr('id');
                    debugger
                        http.delete(this.baseUrl + id, function () {
                        that.load();
                    }, function () {
                        swal('Error', 'Remove agent failed', "error");
                    });
                });
            }
        }
    });
});