import Vue from 'vue'
import Vant from 'vant'
import 'vant/lib/index.css'
import Router from 'vue-router'
import ws from '@/components/WebSocket'

Vue.use(Router)
Vue.use(Vant)

export default new Router({
  routes: [{
    path: '/',
    name: 'ws',
    component: ws
  }]
})
