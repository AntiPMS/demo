<template>
  <div>
    <div style="height: 350px; background-color:#000">
      <van-list v-model="loading"
                :finished="finished"
                finished-text="没有更多了"
                direction="up"
                border=false
                @load="onLoadMsg">
        <van-cell v-for="msgtext in receiveMsgText"
                  :key="msgtext.Id">
          <a v-if="msgtext.isSelf==true">
            {{msgtext.msg}}
          </a>
          <a v-else>
            {{msgtext.senderName}}>>{{msgtext.msg}}
          </a>
        </van-cell>
      </van-list>
      <!-- <van-field v-model="receiveMsgText"
                 type="textarea"
                 style="height: 350px;"
                 autosize
                 readonly /> -->
    </div>
    <div ref="container"
         style="height: 150px;">
      <van-sticky :container="container">
        <van-cell-group>
          <van-cell>
            <van-row>
              <!-- <van-col span="8">
                <van-field v-model="sendMsg.senderId"
                           label="用户主键："
                           disabled />
              </van-col> -->
              <van-col span="12">
                <van-field v-model="sendMsg.senderName"
                           label="用户名：" />
              </van-col>
              <van-col span="4">
                <van-field v-model="sendMsg.targetId"
                           label="门诊号："
                           readonly />
              </van-col>
            </van-row>
          </van-cell>
          <van-cell>
            <van-row>
              <van-col span="4">
                <van-icon name="smile-o"
                          size="35px" />
              </van-col>
              <van-col span="4">
                <van-icon name="photo-o"
                          size="35px" />
              </van-col>
              <van-col span="4">
                <van-icon name="orders-o"
                          size="35px" />
              </van-col>
              <!-- <van-col>
                <van-switch v-model="isEnterSend"
                            size="12px"
                            @input="onInput" />
              </van-col> -->
            </van-row>
          </van-cell>
          <van-cell>
            <van-row>
              <van-col>
                <van-field v-model="sendMsg.msg"
                           v-on:keyup.enter="doSend(true)"
                           type="textarea"
                           label="输入信息"
                           maxlength="150"
                           show-word-limit
                           placeholder="回车发送" />
              </van-col>
              <van-col>
                <van-button v-on:click="doSend(true)"
                            type="info">发送</van-button>
              </van-col>
            </van-row>
          </van-cell>
        </van-cell-group>
      </van-sticky>
    </div>
  </div>
</template>

<script>
export default {
  name: 'MyWebSocket',
  data () {
    return {
      loading: false,
      finished: false,
      container: null,
      isEnterSend: true,
      socket: null,
      receiveMsgText: [],
      sendMsg: {
        Id: '',
        senderId: '',
        senderName: '',
        targetId: '',
        msgType: 1,
        msg: ''
      }
    }
  },
  created: function () {
    // init sendMsg object
    this.sendMsg.senderId = this.guid()
    this.sendMsg.senderName = this.guid().substring(4)
    this.sendMsg.targetId = '1'
  },
  mouted: function () {
    this.container = this.$refs.container
    window.addEventListener('beforeunload', e => {
      debugger
    })
    window.addEventListener('onunload', e => {
      debugger
    })
  },
  beforeDestroy: function () {

  },
  destroy: function () {
    this.socket.onclose()
  },
  methods: {
    onLoadMsg () {
      this.doConnect(this)

      // 加载状态结束
      this.loading = false
      this.finished = true
    },
    onInput (checked) {
      this.isEnterSend = checked
    },
    guid () {
      function S4 () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
      }
      return (S4() + S4())
    },
    doConnect (app) {
      let uri = 'ws://localhost:5100/ws?senderId=' + this.sendMsg.senderId + '&targetId=' + this.sendMsg.targetId
      this.socket = new WebSocket(uri)
      this.socket.onopen = function (e) {
        // app.receiveMsgText += 'opened!\n'
      }
      this.socket.onclose = function (e) {
        // app.receiveMsgText += 'closed!\n'
      }
      this.socket.onmessage = function (e) {
        var rmsg = JSON.parse(e.data)
        app.receiveMsgText.push(
          {
            isSelf: false,
            senderName: rmsg['SenderName'],
            msg: rmsg['Msg']
          })
        // this.socket.close()
      }
      this.socket.onerror = function (e) {
        app.receiveMsgText.push({
          isSelf: false,
          senderName: 'system',
          msg: '连接已断开'
        })
      }
    },
    doSend (isEnterSend) {
      if (isEnterSend) {
        this.sendMsg.msgType = 1
        this.socket.send(JSON.stringify(this.sendMsg))
        this.receiveMsgText.push(
          {
            Id: (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1),
            isSelf: true,
            senderName: this.sendMsg.senderName,
            msg: this.sendMsg.msg
          })
        this.sendMsg.msg = ''
      }
    }
  }
}

</script>
