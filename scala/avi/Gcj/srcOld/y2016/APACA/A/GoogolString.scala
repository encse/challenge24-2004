package y2016.APACA.A

import cmn._

import scala.collection.immutable._
import scala.util.control.Breaks._

object GoogolString extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    def sl(l: BigInt): Stream[BigInt] = l #:: sl(2 * l + 1)

    val k = fetch(PBigInt)-1

    val l = sl(0).find{l => k<l}.get

    def s(l: BigInt, k: BigInt, fInverse: Boolean): Int = {
      if(k == l/2) {
        if (fInverse) 1 else 0
      }else if(k < l/2) {
        s(l / 2, k, fInverse)
      }else {
        s(l/2, l - k - 1, !fInverse)
      }
    }

    out(s(l,k,false))
  }
}
