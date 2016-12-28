package y2016.R1A.B

import cmn._

import scala.collection.mutable

object RankAndFile extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    val n = fetch(PInt)

    val rgrghOrig = (0 until (2 * n - 1)).map(i => fetch(PIndexedSeq(PInt))).toList
    var rgrgh = rgrghOrig

    val rg2h = mutable.ArrayBuffer[List[IndexedSeq[Int]]]()
    var iMissing = -1
    var rghSingle: IndexedSeq[Int] = null
    for (i <- 0 until n) {
      val minh: Int = rgrgh.map(rgh => rgh(i)).min
      val (rgx, rgrghNext) = rgrgh.partition(rgh => rgh(i) == minh)
      assert(rgx.size == 1 || rgx.size == 2)
      assert(rg2h.size == i)
      rg2h.append(rgx)
      if (rgx.size == 1) {
        assert(iMissing == -1)
        iMissing = i
        rghSingle = rgx.head
      }
      rgrgh = rgrghNext
    }

    assert(rg2h.size == n)

    val rghRes = mutable.ArrayBuffer[Int]()

    for ((hh, i) <- rg2h.zipWithIndex) {
      hh match {
        case List(h) => rghRes += h(i)
        case List(h1,h2) => {
          val h1i = h1(iMissing)
          val h2i = h2(iMissing)
          val hSingle = rghSingle(i)
          if (h1i == h2i) {
            assert(h1i == hSingle)
            rghRes += h1i
          } else if (h1i != hSingle) {
            rghRes += h1i
          } else {
            assert(h2i != hSingle)
            rghRes += h2i
          }
        }
      }

    }

    out(rghRes: _*)
  }
}
