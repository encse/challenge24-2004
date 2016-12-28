package y2016.APACA.B

import cmn._

import scala.collection.immutable._
import scala.util.control.Breaks._

object GCube extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    val List(cDim,cCube) = fetch(PList(PInt))

    val rgdim = fetch(PIndexedSeq(PBigDecimal))
    for(icube <- 0 until cCube) {
      val List(idimMin, idimMax) = fetch(PList(PInt))
      val vol = rgdim.slice(idimMin, idimMax+1).fold(BigDecimal(1))(_*_)

      val n = idimMax + 1 - idimMin

      val res = binSearch[BigDecimal](_.pow(n) > vol, 0, 1, 0.0000001)._1

      out(NewLine)
      out(res)
    }
  }
}
