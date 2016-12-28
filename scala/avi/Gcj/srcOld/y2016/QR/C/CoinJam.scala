package y2016.QR.C

import cmn._

import scala.collection.immutable.IndexedSeq
import scala.collection.mutable
import scala.util.Random

object CoinJam extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {
    val List(length, count) = fetch(PList(PInt))

    val x = (length/2)-2

    out(NewLine)
    0.until(count).map(BigInt(_).toString(2)).map(st => "0" * (x - st.length) + st ).map("1" + _ + "1").foreach(half =>{
      val coin = half + half
      out(coin)
      val div = "1" + "0" * x + "01"
      2.to(10).foreach(radix =>{
        val num = BigInt(div, radix)
        out(num)
        val o = BigInt(coin, radix)
        assert(o % num == BigInt(0))
      })
      out(NewLine)
    })


//
//    val rgprim = mutable.ArrayBuffer[BigInt](BigInt(2))
//
//    def getDiv(n: BigInt): Option[BigInt] = {
//      nums(0).map(getPrim(_)).takeWhile(p => p * p <= n).find(p => n % p == 0)
//    }
//
//    def isPrim(n: BigInt): Boolean = {
//      getDiv(n).forall(p => false)
//    }
//
//    def getPrim(i: Int): BigInt = {
//      while (i >= rgprim.size) {
//        rgprim += nums(rgprim.last).find(n => isPrim(n)).get
//      }
//      rgprim(i)
//    }
//
//    val r = new Random()
//
//    val rgres = mutable.ArrayBuffer[(String, IndexedSeq[BigInt])]()
//    val hlmcoin = mutable.HashSet[String]()
//
//    while(rgres.size < count) {
//      val coin = "1" + (2 until length).map(_ => if (r.nextBoolean()) "1" else "0").mkString + "1"
//
//      if(!hlmcoin.contains(coin)) {
//        hlmcoin += coin
//
//        val rgn = 2.to(10).map(radix => BigInt(coin, radix))
//        val rgodiv: IndexedSeq[Option[BigInt]] = 2.to(10).map(radix => getDiv(BigInt(coin, radix)))
//
//        if (rgodiv.forall(_.isDefined))
//          rgres += Tuple2(coin, rgodiv.map(_.get))
//      }
//    }
//
//    out(NewLine)
//    rgres.foreach(res => {
//      out(res._1)
//      res._2.foreach(out(_))
//      out(NewLine)
//    })
  }
}
