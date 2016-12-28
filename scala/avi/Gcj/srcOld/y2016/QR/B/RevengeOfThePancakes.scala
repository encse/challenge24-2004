package y2016.QR.B

import cmn._

import scala.collection.mutable

object RevengeOfThePancakes extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {

    def flip(ch: Char): Char = ch match {
      case '+' => '-'
      case '-' => '+'
    }

    def rot(st: String, n: Int): String = {
      val res = 0.until(st.length).map(i => if (i < n)
        flip(st(n - 1 - i))
      else
        st(i)
      )

      new String(res.toArray)
    }

    def sames(st: String) = Math.max(st.takeWhile(_ == '+').size, st.takeWhile(_ == '-').size)

    def dist(stStart: String) = {
      var d = 0
      var st = stStart
      while (st.exists(_ != '+')) {
        st = rot(st, sames(st))
        d += 1
      }
      d
    }

    out(dist(fetch(PString)))

//    def nexts(st: String) = {
//      1.to(st.length).map(c => rot(st, c))
//    }
//
//    val mpdistBySt = mutable.HashMap[String, Int]()
//
//    1.to(100).foreach(length => {
//      println(length)
//      val st0 = "+" * length
//
//      //val hlmSt = mutable.HashSet[String](st0)
//      val quSt = mutable.Queue[String](st0)
//      mpdistBySt.clear()
//      mpdistBySt += st0 -> 0
//
//      while (quSt.nonEmpty) {
//        //assert(quSt.size == hlmSt.size)
//        val cur = quSt.dequeue()
//        //println(s"$cur (${hlmSt.size}): ")
//        //assert(hlmSt.contains(cur))
//        //hlmSt.remove(cur)
//        val dist = mpdistBySt(cur) + 1
//        nexts(cur).filterNot(mpdistBySt.contains).foreach(next => {
//          mpdistBySt += next -> dist
//          //assert(!hlmSt.contains(next))
//          quSt.enqueue(next)
//        })
//      }
//
//      mpdistBySt.toList.foreach({ case (stStart, d) =>
//
//        assert(d == dist(stStart))
//      })
//    })
  }
}
