$(function () {
    new Vue({
        el: '#app',
        data: {
            page: 1,
            limit: 15,
            count: 0,
            items: []
        },
        mounted: function () {
            this.load();
        },
        methods: {
            load: function () {
                let that = this;
                let data = window.location.href.split('/');
                let id = data[data.length - 2];
                debugger
                let pagedQuery = getPagedQuery();
                http.get(`/crawler/api/v1.0/agents/${id}/heartbeats?${pagedQuery}`, function (result) {
                    that.page = result.page;
                    that.limit = result.pageSize;
                    that.count = result.totalCount;
                    that.items = [];
                    result.data.forEach(x => {
                        that.items.push(x);
                    });
                });
            }
        }
    });
});