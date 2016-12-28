package y2016.QR.D

import cmn._

object Fractiles extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    val List(k, c, s) = fetch(PList(PInt))
    if (c * s < k) {
      out("IMPOSSIBLE")
      return
    }

    var ic = 0
    var p = BigInt(0)
    (0 until k).foreach(i => {
      p = (p * k) + i
      ic += 1
      if (ic == c) {
        out(p + 1)
        ic = 0
        p = 0
      }
    })
    if (ic > 0)
      out(p + 1)
  }
}
